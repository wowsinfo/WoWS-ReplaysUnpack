using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nodsoft.WowsReplaysUnpack.Core.Entities;
public enum EntityType
{
	Client = 1,
	Cell = 2,
	Base = 4
}
public class Entity
{
	private readonly ILogger<Entity> _logger;

	private readonly EntityDefinition _entityDefinition;

	private readonly Dictionary<string, MethodInfo> _methodSubscriptions;
	private readonly Dictionary<string, MethodInfo> _propertyChangedSubscriptions;

	private readonly Dictionary<int, PropertyDefinition> _clientPropertyDefinitions;
	private readonly Dictionary<int, PropertyDefinition> _clientPropertyInternalDefinitions;
	private readonly Dictionary<int, PropertyDefinition> _cellPropertyDefinitions;
	private readonly Dictionary<int, PropertyDefinition> _basePropertyDefinitions;
	public int Id { get; }
	public string Name { get; }

	/// <summary>
	/// Is in Area of Influence (visible area)
	/// </summary>
	public bool IsInAoI { get; }

	public Dictionary<string, object?> ClientProperties { get; } = new();
	public Dictionary<string, object?> CellProperties { get; } = new();
	public Dictionary<string, object?> BaseProperties { get; } = new();
	public Dictionary<string, object> VolatileProperties { get; } = new();
	public List<EntityMethodDefinition> MethodDefinitions => _entityDefinition.ClientMethods;

	public Entity(int id, string name, EntityDefinition entityDefinition,
		Dictionary<string, MethodInfo> methodSubscriptions,
		Dictionary<string, MethodInfo> propertyChangedSubscriptions,
		ILogger<Entity> logger)
	{
		Id = id;
		Name = name;
		_entityDefinition = entityDefinition;
		_methodSubscriptions = methodSubscriptions;
		_propertyChangedSubscriptions = propertyChangedSubscriptions;
		_logger = logger;
		VolatileProperties = _entityDefinition.VolatileProperties.ToDictionary(kv => kv.Key, kv => kv.Value);

		_clientPropertyDefinitions = GetPropertiesByFlags(EntityFlag.ALL_CLIENTS | EntityFlag.BASE_AND_CLIENT | EntityFlag.OTHER_CLIENTS
			| EntityFlag.OWN_CLIENT | EntityFlag.CELL_PUBLIC_AND_OWN, true);

		_clientPropertyInternalDefinitions = GetPropertiesByFlags(EntityFlag.ALL_CLIENTS | EntityFlag.OTHER_CLIENTS
			| EntityFlag.OWN_CLIENT | EntityFlag.CELL_PUBLIC_AND_OWN);

		_cellPropertyDefinitions = GetPropertiesByFlags(EntityFlag.CELL_PUBLIC_AND_OWN | EntityFlag.CELL_PUBLIC);

		_basePropertyDefinitions = GetPropertiesByFlags(EntityFlag.BASE_AND_CLIENT);
	}

	private Dictionary<int, PropertyDefinition> GetPropertiesByFlags(EntityFlag flags, bool orderBySize = false)
		=> _entityDefinition.GetPropertiesByFlags(flags, orderBySize).Select((p, i) => new { Index = i, Definition = p }).ToDictionary(p => p.Index, p => p.Definition);


	internal void CallClientMethod(int index, BinaryReader reader, object? subscriptionTarget)
	{
		if (subscriptionTarget is null)
			return;

		var methodDefinition = MethodDefinitions.ElementAtOrDefault(index);
		if (methodDefinition is null)
		{
			_logger.LogError("Method with index {index} was not found on entity with name {Name} ({Id})", index, Name, Id);
			return;
		}
		var hash = $"{Name}_{methodDefinition.Name}";
		if (_methodSubscriptions.TryGetValue(hash, out var methodInfo))
		{
			var methodParameters = methodInfo.GetParameters();
			if (methodDefinition.Arguments.Count != methodParameters.Length - 1
				|| methodParameters[0].ParameterType != typeof(Entity)
				|| !methodDefinition.Arguments.Select(a => a.DataType.ClrType).SequenceEqual(methodParameters.Skip(1).Select(m => m.ParameterType))
			)
			{
				_logger.LogError("Arguments of method definition and method subscription does not match");
				return;
			}
			try
			{
				var methodArgumentValues = methodDefinition.Arguments.Select(a => a.GetValue(reader))
					.Prepend(this).ToArray();
				_logger.LogDebug("Calling method subscription with hash {hash}", hash);
				methodInfo.Invoke(subscriptionTarget, methodArgumentValues);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error when calling method subscription with hash {hash}", hash);
			}
		}
	}

	internal void SetClientProperty(int exposedIndex, BinaryReader reader, object? subscriptionTarget)
	{
		_logger.LogDebug("Setting client property with index {index} on entity {Name} ({id})", exposedIndex, Name, Id);
		var propertyDefinition = _clientPropertyDefinitions[exposedIndex];
		var propertyValue = propertyDefinition.GetValue(reader, propertyDefinition.XmlNode);
		ClientProperties[propertyDefinition.Name] = propertyValue;

		if (subscriptionTarget is null)
			return;

		var hash = $"{Name}_{propertyDefinition.Name}";
		if (_propertyChangedSubscriptions.TryGetValue(hash, out var methodInfo))
		{
			var methodParameters = methodInfo.GetParameters();
			if (methodParameters.Length != 2
					|| methodParameters[0].ParameterType != typeof(Entity)
					|| methodParameters[1].ParameterType != propertyDefinition.DataType.ClrType
			)
			{
				_logger.LogError("Arguments of property definition and property changed subscription does not match");
				return;
			}
			try
			{
				_logger.LogDebug("Calling property changed subscription with hash {hash}", hash);
				methodInfo.Invoke(subscriptionTarget, new[] { this, propertyValue });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error when calling property changed subscription with hash {hash}", hash);
			}
		}
	}

	internal void SetClientPropertyInternal(int internalIndex, BinaryReader reader)
	{
		_logger.LogDebug("Setting internal client property with index {index} on entity {Name} ({id})", internalIndex, Name, Id);
		var propertyDefinition = _clientPropertyInternalDefinitions[internalIndex];
		var propertyValue = propertyDefinition.GetValue(reader, propertyDefinition.XmlNode);
		ClientProperties[propertyDefinition.Name] = propertyValue;
	}

	internal void SetCellProperty(int internalIndex, BinaryReader reader)
	{
		_logger.LogDebug("Setting cell property with index {index} on entity {Name} ({id})", internalIndex, Name, Id);
		var propertyDefinition = _cellPropertyDefinitions[internalIndex];
		var propertyValue = propertyDefinition.GetValue(reader, propertyDefinition.XmlNode);
		CellProperties[propertyDefinition.Name] = propertyValue;
	}

	internal void SetBaseProperty(int internalIndex, BinaryReader reader)
	{
		_logger.LogDebug("Setting base property with index {index} on entity {Name} ({id})", internalIndex, Name, Id);
		var propertyDefinition = _basePropertyDefinitions[internalIndex];
		var propertyValue = propertyDefinition.GetValue(reader, propertyDefinition.XmlNode);
		BaseProperties[propertyDefinition.Name] = propertyValue;
	}
}
