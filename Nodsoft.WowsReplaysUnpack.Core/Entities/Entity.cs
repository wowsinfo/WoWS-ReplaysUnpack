using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Exceptions;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace Nodsoft.WowsReplaysUnpack.Core.Entities;
public enum EntityType
{
	Client = 1,
	Cell = 2,
	Base = 4
}
public class Entity
{
	protected ILogger<Entity> Logger { get; }

	protected EntityDefinition EntityDefinition { get; }

	protected Dictionary<string, MethodInfo[]> MethodSubscriptions { get; }
	protected Dictionary<string, MethodInfo[]> PropertyChangedSubscriptions { get; }

	protected PropertyDefinition[] ClientPropertyDefinitions { get; }
	protected PropertyDefinition[] InternalClientPropertyDefinitions { get; }
	protected PropertyDefinition[] CellPropertyDefinitions { get; }
	protected PropertyDefinition[] BasePropertyDefinitions { get; }

	public uint Id { get; }
	public string Name { get; }

	/// <summary>
	/// Is in Area of Influence (visible area)
	/// </summary>
	public bool IsInAoI { get; set; }

	public Dictionary<string, object?> ClientProperties { get; } = new();
	public Dictionary<string, object?> CellProperties { get; } = new();
	public Dictionary<string, object?> BaseProperties { get; } = new();
	public Dictionary<string, object> VolatileProperties { get; } = new();
	public List<EntityMethodDefinition> MethodDefinitions => EntityDefinition.ClientMethods;


	public Vector3 VPosition
	{
		get => VolatileProperties.ContainsKey("position") ? (Vector3)VolatileProperties["position"] : new Vector3();
		set => VolatileProperties["position"] = value;
	}

	public float Yaw
	{
		get => VolatileProperties.ContainsKey("yaw") ? (float)VolatileProperties["yaw"] : 0f;
		set => VolatileProperties["yaw"] = value;
	}

	public float Pitch
	{
		get => VolatileProperties.ContainsKey("pitch") ? (float)VolatileProperties["pitch"] : 0f;
		set => VolatileProperties["pitch"] = value;
	}

	public float Roll
	{
		get => VolatileProperties.ContainsKey("roll") ? (float)VolatileProperties["roll"] : 0f;
		set => VolatileProperties["roll"] = value;
	}

	public Entity(uint id, string name, EntityDefinition entityDefinition,
		Dictionary<string, MethodInfo[]> methodSubscriptions,
		Dictionary<string, MethodInfo[]> propertyChangedSubscriptions,
		ILogger<Entity> logger)
	{
		Id = id;
		Name = name;
		EntityDefinition = entityDefinition;
		MethodSubscriptions = methodSubscriptions;
		PropertyChangedSubscriptions = propertyChangedSubscriptions;
		Logger = logger;
		VolatileProperties = EntityDefinition.VolatileProperties.ToDictionary(kv => kv.Key, kv => kv.Value);

		ClientPropertyDefinitions = EntityDefinition.GetPropertiesByFlags(EntityFlag.ALL_CLIENTS | EntityFlag.BASE_AND_CLIENT | EntityFlag.OTHER_CLIENTS
			| EntityFlag.OWN_CLIENT | EntityFlag.CELL_PUBLIC_AND_OWN, true);

		InternalClientPropertyDefinitions = EntityDefinition.GetPropertiesByFlags(EntityFlag.ALL_CLIENTS | EntityFlag.OTHER_CLIENTS
			| EntityFlag.OWN_CLIENT | EntityFlag.CELL_PUBLIC_AND_OWN);

		CellPropertyDefinitions = EntityDefinition.GetPropertiesByFlags(EntityFlag.CELL_PUBLIC_AND_OWN | EntityFlag.CELL_PUBLIC);

		BasePropertyDefinitions = EntityDefinition.GetPropertiesByFlags(EntityFlag.BASE_AND_CLIENT);
	}

	public string? GetClientPropertyNameForIndex(int index)
		=> ClientPropertyDefinitions.ElementAtOrDefault(index)?.Name;

	public string? GetClientMethodNameForIndex(uint index)
		=> MethodDefinitions.ElementAtOrDefault((int)index)?.Name;

	public virtual void CallClientMethod(uint index, BinaryReader reader, object? subscriptionTarget)
	{
		if (subscriptionTarget is null)
			return;

		EntityMethodDefinition? methodDefinition = MethodDefinitions.ElementAtOrDefault((int)index);
		if (methodDefinition is null)
		{
			Logger.LogError("Method with index {index} was not found on entity with name {Name} ({Id})", index, Name, Id);
			return;
		}
		string hash = $"{Name}_{methodDefinition.Name}";
		if (MethodSubscriptions.TryGetValue(hash, out MethodInfo[]? methodInfos))
		{
			foreach (var methodInfo in methodInfos)
			{
				var attribute = methodInfo.GetCustomAttribute<MethodSubscriptionAttribute>()!;
				if (attribute.ParamsAsDictionary)
					CallClientMethodWithDictionary(reader, subscriptionTarget, methodDefinition, hash, methodInfo);
				else
					CallClientMethodWithParameters(reader, subscriptionTarget, methodDefinition, hash, methodInfo);
			}
		}
	}

	private void CallClientMethodWithParameters(BinaryReader reader, object? subscriptionTarget,
		EntityMethodDefinition methodDefinition, string hash, MethodInfo methodInfo)
	{
		ParameterInfo[] methodParameters = methodInfo.GetParameters();
		if (methodDefinition.Arguments.Count != methodParameters.Length - 1
			|| methodParameters[0].ParameterType != typeof(Entity)
			|| !methodDefinition.Arguments.Select(a => a.DataType.ClrType).SequenceEqual(methodParameters.Skip(1).Select(m => m.ParameterType))
		)
		{
			StringBuilder sb = new StringBuilder("Arguments of method definition and method subscription do not match")
						.AppendLine()
						.Append("Method Name: ").AppendLine(methodDefinition.Name)
						.Append("Subscription Name: ").AppendLine(methodInfo.Name)
					  .Append("Expected Arguments: ")
							.AppendLine(string.Join(", ", methodDefinition.Arguments.Select(a => $"{a.DataType.ClrType.Name} {a.Name}").Prepend("Entity entity")))
					  .Append("Actual Parameters: ")
							.AppendLine(string.Join(", ", methodParameters.Select(a => $"{a.ParameterType.Name} {a.Name}")));
			Logger.LogError(sb.ToString());
			return;
		}
		try
		{
			object?[] methodArgumentValues = methodDefinition.Arguments.Select(a => a.GetValue(reader))
				.Prepend(this).ToArray();
			Logger.LogDebug("Calling method subscription with hash {hash}", hash);
			methodInfo.Invoke(subscriptionTarget, methodArgumentValues);
		}
		catch (Exception ex)
		{
			if (ex.InnerException is CVESecurityException cveEx)
				throw ex.InnerException;
			Logger.LogError(ex, "Error when calling method subscription with hash {hash}", hash);
		}
	}

	private void CallClientMethodWithDictionary(BinaryReader reader, object? subscriptionTarget,
		EntityMethodDefinition methodDefinition, string hash, MethodInfo methodInfo)
	{
		ParameterInfo[] methodParameters = methodInfo.GetParameters();
		if (methodParameters.Length != 2
			|| methodParameters[0].ParameterType != typeof(Entity)
			|| methodParameters[1].ParameterType?.GetGenericTypeDefinition() != typeof(Dictionary<,>)
			|| methodParameters[1].ParameterType.GenericTypeArguments.Length != 2
			|| methodParameters[1].ParameterType.GenericTypeArguments[0] != typeof(string)
			|| methodParameters[1].ParameterType.GenericTypeArguments[1] != typeof(object)

		)
		{
			StringBuilder sb = new StringBuilder("Arguments of method definition and method subscription do not match")
						.AppendLine()
						.Append("Method Name: ").AppendLine(methodDefinition.Name)
						.Append("Subscription Name: ").AppendLine(methodInfo.Name)
					  .AppendLine("Expected Arguments: Entity entity, Dictionary<string, object> arguments")
					  .Append("Actual Parameters: ")
							.AppendLine(string.Join(", ", methodParameters.Select(a => $"{a.ParameterType.Name} {a.Name}")));
			Logger.LogError(sb.ToString());
			return;
		}
		try
		{
			object[] methodArgumentValues = new object[] { this, methodDefinition.Arguments.ToDictionary(a => a.Name, a => a.GetValue(reader)) };
			Logger.LogDebug("Calling method subscription with hash {hash}", hash);
			methodInfo.Invoke(subscriptionTarget, methodArgumentValues);
		}
		catch (Exception ex)
		{
			if (ex.InnerException is CVESecurityException cveEx)
				throw ex.InnerException;
			Logger.LogError(ex, "Error when calling method subscription with hash {hash}", hash);
		}
	}

	public virtual void SetClientProperty(uint exposedIndex, BinaryReader reader, object? subscriptionTarget)
	{
		Logger.LogDebug("Setting client property with index {index} on entity {Name} ({id})", exposedIndex, Name, Id);
		PropertyDefinition propertyDefinition = ClientPropertyDefinitions[exposedIndex];
		object? propertyValue = propertyDefinition.GetValue(reader, propertyDefinition.XmlNode);
		ClientProperties[propertyDefinition.Name] = propertyValue;

		if (subscriptionTarget is null)
			return;

		string hash = $"{Name}_{propertyDefinition.Name}";
		if (PropertyChangedSubscriptions.TryGetValue(hash, out MethodInfo[]? methodInfos))
		{
			foreach (var methodInfo in methodInfos)
			{
				ParameterInfo[] methodParameters = methodInfo.GetParameters();
				if (methodParameters.Length != 2
						|| methodParameters[0].ParameterType != typeof(Entity)
						|| methodParameters[1].ParameterType != propertyDefinition.DataType.ClrType
				)
				{
					StringBuilder sb = new StringBuilder("Arguments of property definition and property changed subscription does not match")
								.AppendLine()
								.Append("Property Name: ").AppendLine(propertyDefinition.Name)
								.Append("Subscription Name: ").AppendLine(methodInfo.Name)
							  .Append("Expected Arguments: ")
									.AppendLine($"Entity entity, {propertyDefinition.DataType.ClrType.Name} value")
							  .Append("Actual Parameters: ")
									.AppendLine(string.Join(", ", methodParameters.Select(a => $"{a.ParameterType.Name} {a.Name}")));
					Logger.LogError(sb.ToString());
					return;
				}
				try
				{
					Logger.LogDebug("Calling property changed subscription with hash {hash}", hash);
					methodInfo.Invoke(subscriptionTarget, new[] { this, propertyValue });
				}
				catch (Exception ex)
				{
					Logger.LogError(ex, "Error when calling property changed subscription with hash {hash}", hash);
				}
			}
		}
	}

	public virtual void SetInternalClientProperty(int internalIndex, BinaryReader reader)
	{
		Logger.LogDebug("Setting internal client property with index {index} on entity {Name} ({id})", internalIndex, Name, Id);
		PropertyDefinition propertyDefinition = InternalClientPropertyDefinitions[internalIndex];
		object? propertyValue = propertyDefinition.GetValue(reader, propertyDefinition.XmlNode);
		ClientProperties[propertyDefinition.Name] = propertyValue;
	}

	public virtual void SetCellProperty(int internalIndex, BinaryReader reader)
	{
		Logger.LogDebug("Setting cell property with index {index} on entity {Name} ({id})", internalIndex, Name, Id);
		PropertyDefinition propertyDefinition = CellPropertyDefinitions[internalIndex];
		object? propertyValue = propertyDefinition.GetValue(reader, propertyDefinition.XmlNode);
		CellProperties[propertyDefinition.Name] = propertyValue;
	}

	public virtual void SetBaseProperty(int internalIndex, BinaryReader reader)
	{
		Logger.LogDebug("Setting base property with index {index} on entity {Name} ({id})", internalIndex, Name, Id);
		PropertyDefinition propertyDefinition = BasePropertyDefinitions[internalIndex];
		object? propertyValue = propertyDefinition.GetValue(reader, propertyDefinition.XmlNode);
		BaseProperties[propertyDefinition.Name] = propertyValue;
	}

	public virtual void SetBaseProperties(BinaryReader reader)
	{
		for (int i = 0; i < BasePropertyDefinitions.Length; i++)
			SetBaseProperty(i, reader);
	}

	public virtual void SetInternalClientProperties(BinaryReader reader)
	{
		for (int i = 0; i < InternalClientPropertyDefinitions.Length; i++)
			SetInternalClientProperty(i, reader);
	}

	public void SetPosition(PositionContainer position)
	{
		VPosition = position.Position;
		Yaw = position.Yaw;
		Pitch = position.Pitch;
		Roll = position.Roll;
	}

	public PositionContainer GetPosition()
		=> new(VPosition, Yaw, Pitch, Roll);

	public override string ToString() => $"{Name} <{Id}>";
}
