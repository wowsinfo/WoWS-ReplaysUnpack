using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Exceptions;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace Nodsoft.WowsReplaysUnpack.Core.Entities;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")] // Most elements are exposed to usersapce, so shouldn't be restricted past public/protected.
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
	public Dictionary<string, object> VolatileProperties { get; }
	public IEnumerable<EntityMethodDefinition> MethodDefinitions => EntityDefinition.ClientMethods;


	public Vector3 VPosition
	{
		get => VolatileProperties.ContainsKey("position") ? (Vector3)VolatileProperties["position"] : new();
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

		ClientPropertyDefinitions = EntityDefinition.GetPropertiesByFlags(EntityFlag.ALL_CLIENTS
			| EntityFlag.BASE_AND_CLIENT
			| EntityFlag.OTHER_CLIENTS
			| EntityFlag.OWN_CLIENT
			| EntityFlag.CELL_PUBLIC_AND_OWN, true
		);

		InternalClientPropertyDefinitions = EntityDefinition.GetPropertiesByFlags(EntityFlag.ALL_CLIENTS
			| EntityFlag.OTHER_CLIENTS
			| EntityFlag.OWN_CLIENT
			| EntityFlag.CELL_PUBLIC_AND_OWN
		);

		CellPropertyDefinitions = EntityDefinition.GetPropertiesByFlags(EntityFlag.CELL_PUBLIC_AND_OWN | EntityFlag.CELL_PUBLIC);

		BasePropertyDefinitions = EntityDefinition.GetPropertiesByFlags(EntityFlag.BASE_AND_CLIENT);
	}

	public string? GetClientPropertyNameForIndex(int index)
		=> ClientPropertyDefinitions.ElementAtOrDefault(index)?.Name;

	public string? GetClientMethodNameForIndex(uint index)
		=> MethodDefinitions.ElementAtOrDefault((int)index)?.Name;

	public virtual void CallClientMethod(uint index, float packetTime, BinaryReader reader, object? subscriptionTarget)
	{
		if (subscriptionTarget is null)
		{
			return;
		}

		EntityMethodDefinition? methodDefinition = MethodDefinitions.ElementAtOrDefault((int)index);

		if (methodDefinition is null)
		{
			Logger.LogError("Method with index {index} was not found on entity with name {Name} ({Id})", index, Name, Id);

			return;
		}

		string hash = $"{Name}_{methodDefinition.Name}";

		if (MethodSubscriptions.TryGetValue(hash, out MethodInfo[]? methodInfos))
		{
			foreach (MethodInfo methodInfo in methodInfos)
			{
				MethodSubscriptionAttribute attribute = methodInfo.GetCustomAttribute<MethodSubscriptionAttribute>()!;

				if (attribute.ParamsAsDictionary)
				{
					CallClientMethodWithDictionary(reader, packetTime, subscriptionTarget, methodDefinition, hash, methodInfo, attribute);
				}
				else
				{
					CallClientMethodWithParameters(reader, packetTime, subscriptionTarget, methodDefinition, hash, methodInfo, attribute);
				}

				reader.BaseStream.Seek(0, SeekOrigin.Begin);
			}
		}
	}

	private void CallClientMethodWithParameters(BinaryReader reader, float packetTime, object? subscriptionTarget,
		EntityMethodDefinition methodDefinition, string hash, MethodInfo methodInfo, MethodSubscriptionAttribute attribute)
	{
		if (!ValidateParameterTypes(methodDefinition, methodInfo, attribute))
		{
			return;
		}

		try
		{
			IEnumerable<object?> methodArgumentValues = methodDefinition.Arguments.Select(a => a.GetValue(reader));

			if (attribute.IncludeEntity)
			{
				methodArgumentValues = methodArgumentValues.Prepend(this);
			}

			if (attribute.IncludePacketTime)
			{
				methodArgumentValues = methodArgumentValues.Prepend(packetTime);
			}

			Logger.LogDebug("Calling method subscription with hash {hash}", hash);
			methodInfo.Invoke(subscriptionTarget, methodArgumentValues.ToArray());
		}
		catch (Exception ex)
		{
			if (ex.InnerException is CveSecurityException)
			{
				throw ex.InnerException;
			}

			Logger.LogError(ex, "Error when calling method subscription with hash {hash}", hash);
		}
	}

	private void CallClientMethodWithDictionary(BinaryReader reader, float packetTime, object? subscriptionTarget,
		EntityMethodDefinition methodDefinition, string hash, MethodInfo methodInfo, MethodSubscriptionAttribute attribute)
	{
		if (!ValidateParameterTypes(methodDefinition, methodInfo, attribute))
		{
			return;
		}

		try
		{
			IEnumerable<object> methodArgumentValues = new object[] { methodDefinition.Arguments.ToDictionary(a => a.Name, a => a.GetValue(reader)) };

			if (attribute.IncludeEntity)
			{
				methodArgumentValues = methodArgumentValues.Prepend(this);
			}

			if (attribute.IncludePacketTime)
			{
				methodArgumentValues = methodArgumentValues.Prepend(packetTime);
			}

			Logger.LogDebug("Calling method subscription with hash {hash}", hash);
			methodInfo.Invoke(subscriptionTarget, methodArgumentValues.ToArray());
		}
		catch (Exception ex)
		{
			if (ex.InnerException is CveSecurityException)
			{
				throw ex.InnerException;
			}

			Logger.LogError(ex, "Error when calling method subscription with hash {hash}", hash);
		}
	}

	private bool ValidateParameterTypes(EntityMethodDefinition methodDefinition, MethodBase methodInfo, MethodSubscriptionAttribute attribute)
	{
		Type[] actualParameterTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
		var expectedParameterTypes = new[]
		{
			attribute.IncludeEntity ? new { Type = typeof(Entity), Name = "entity" } : null, 
			attribute.IncludePacketTime ? new { Type = typeof(float), Name = "packetTime" } : null
		};

		if (attribute.ParamsAsDictionary)
		{
			expectedParameterTypes = expectedParameterTypes.Append(new { Type = typeof(Dictionary<string, object?>), Name = "arguments" })
				.Where(t => t is not null)
				.ToArray();
		}
		else
		{
			expectedParameterTypes = expectedParameterTypes
				.Concat(methodDefinition.Arguments.Select(a => new { Type = a.DataType.ClrType, a.Name }))
				.Where(t => t is not null)
				.ToArray();
		}

		if (!actualParameterTypes.SequenceEqual(expectedParameterTypes.Select(t => t!.Type)))
		{
			StringBuilder sb = new StringBuilder("Arguments of method definition and method subscription do not match")
				.AppendLine()
				.Append("Method Name: ")
				.AppendLine(methodDefinition.Name)
				.Append("Subscription Name: ")
				.AppendLine(methodInfo.Name)
				.Append("Expected Arguments: ")
				.AppendLine(string.Join(", ", expectedParameterTypes.Select((t, i) => $"{t!.Type.Name} {t.Name}")))
				.Append("Actual Parameters: ")
				.AppendLine(string.Join(", ", methodInfo.GetParameters().Select(a => $"{a.ParameterType.Name} {a.Name}")));
			Logger.LogError(sb.ToString());

			return false;
		}

		return true;
	}

	public virtual void SetClientProperty(uint exposedIndex, BinaryReader reader, object? subscriptionTarget)
	{
		Logger.LogDebug("Setting client property with index {index} on entity {Name} ({id})", exposedIndex, Name, Id);
		PropertyDefinition propertyDefinition = ClientPropertyDefinitions[exposedIndex];
		object? propertyValue = propertyDefinition.GetValue(reader, propertyDefinition.XmlNode);
		ClientProperties[propertyDefinition.Name] = propertyValue;

		if (subscriptionTarget is null)
		{
			return;
		}

		string hash = $"{Name}_{propertyDefinition.Name}";

		if (PropertyChangedSubscriptions.TryGetValue(hash, out MethodInfo[]? methodInfos))
		{
			foreach (MethodInfo methodInfo in methodInfos)
			{
				ParameterInfo[] methodParameters = methodInfo.GetParameters();

				if (methodParameters.Length is not 2
					|| methodParameters[0].ParameterType != typeof(Entity)
					|| methodParameters[1].ParameterType != propertyDefinition.DataType.ClrType
					)
				{
					StringBuilder sb = new StringBuilder("Arguments of property definition and property changed subscription does not match")
						.AppendLine()
						.Append("Property Name: ")
						.AppendLine(propertyDefinition.Name)
						.Append("Subscription Name: ")
						.AppendLine(methodInfo.Name)
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
		{
			SetBaseProperty(i, reader);
		}
	}

	public virtual void SetInternalClientProperties(BinaryReader reader)
	{
		for (int i = 0; i < InternalClientPropertyDefinitions.Length; i++)
		{
			SetInternalClientProperty(i, reader);
		}
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