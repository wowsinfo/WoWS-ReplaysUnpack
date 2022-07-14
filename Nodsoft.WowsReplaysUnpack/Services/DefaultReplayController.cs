using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Entities;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Nodsoft.WowsReplaysUnpack.Services;

public class DefaultReplayController : IReplayController
{
	private static readonly Dictionary<string, MethodInfo> _methodSubscriptions;
	private static readonly Dictionary<string, MethodInfo> _propertyChangedSubscriptions;
	private readonly IDefinitionStore _definitionStore;
	private readonly ILogger<Entity> _entityLogger;

	public virtual UnpackedReplay? Replay { get; protected set; }

	static DefaultReplayController()
	{
		_methodSubscriptions = typeof(DefaultReplayController).GetMethods()
			.Select(m => new { Attribute = m.GetCustomAttribute<MethodSubscriptionAttribute>(), MethodInfo = m })
			.Where(m => m.Attribute is not null)
			.ToDictionary(m => $"{m.Attribute!.EntityName}_{m.Attribute.MethodName}", m => m.MethodInfo);

		_propertyChangedSubscriptions = typeof(DefaultReplayController).GetMethods()
			.Select(m => new { Attribute = m.GetCustomAttribute<PropertyChangedSubscriptionAttribute>(), MethodInfo = m })
			.Where(m => m.Attribute is not null)
			.ToDictionary(m => $"{m.Attribute!.EntityName}_{m.Attribute.PropertyName}", m => m.MethodInfo);
	}

	public DefaultReplayController(IDefinitionStore definitionStore,
		ILogger<Entity> entityLogger)
	{
		_definitionStore = definitionStore;
		_entityLogger = entityLogger;
	}

	public virtual UnpackedReplay CreateUnpackedReplay(ArenaInfo arenaInfo)
	{
		Replay = new UnpackedReplay(arenaInfo);
		return Replay!;
	}

	public virtual void HandleNetworkPacket(INetworkPacket networkPacket)
	{
		if (networkPacket is MapPacket mapPacket)
			OnMap(mapPacket);
		else if (networkPacket is BasePlayerCreatePacket _1)
			OnBasePlayerCreate(_1);
		else if (networkPacket is CellPlayerCreatePacket _2)
			OnCellPlayerCreate(_2);
		else if (networkPacket is EntityEnterPacket _3)
			Replay!.Entities[_3.EntityId].IsInAoI = true;
		else if (networkPacket is EntityLeavePacket _4)
			Replay!.Entities[_4.EntityId].IsInAoI = false;
		else if (networkPacket is EntityCreatePacket _5)
			OnEntityCreate(_5);
		else if (networkPacket is PositionPacket _6)
			OnPosition(_6);
		else if (networkPacket is PlayerPositionPacket _7)
			OnPlayerPosition(_7);
		else if (networkPacket is EntityMethodPacket _8)
			OnEntityMethod(_8);
		else if (networkPacket is EntityPropertyPacket _9)
			OnEntityProperty(_9);
		else if (networkPacket is NestedPropertyPacket _10)
			OnNestedProperty(_10);
	}

	public virtual void OnMap(MapPacket packet)
	{

	}
	public virtual void OnBasePlayerCreate(BasePlayerCreatePacket packet)
	{
		Replay!.Entities.GetOrAddValue(packet.EntityId, out var entity, () => CreateEntity(packet.EntityId, "Avatar"));

		using var binaryReader = packet.Data.GetBinaryReader();
		entity.SetBaseProperties(binaryReader);
		Replay!.PlayerEntityId = packet.EntityId;
	}

	public virtual void OnCellPlayerCreate(CellPlayerCreatePacket packet)
	{
		Replay!.Entities.GetOrAddValue(packet.EntityId, out var entity, () => CreateEntity(packet.EntityId, "Avatar"));

		using var binaryReader = packet.Data.GetBinaryReader();
		entity.SetInternalClientProperties(binaryReader);
	}

	public virtual void OnEntityCreate(EntityCreatePacket packet)
	{
		var entity = CreateEntity(packet.EntityId, packet.Type);
		using BinaryReader binaryReader = packet.Data.GetBinaryReader();
		var valuesCount = binaryReader.ReadByte();
		foreach (var i in Enumerable.Range(0, valuesCount))
		{
			var propertyIndexSize = binaryReader.ReadByte();
			var propertyIndex = binaryReader.ReadBytes(propertyIndexSize)[0];
			entity.SetClientProperty(propertyIndex, binaryReader, this);
		}
	}

	public virtual void OnPosition(PositionPacket packet)
	{
		var entity = Replay!.Entities[packet.EntityId];
		entity.SetPosition(packet.Position);
	}

	public virtual void OnPlayerPosition(PlayerPositionPacket packet)
	{
		/* 
		 Entity at ID 1 is the primary one's position being updated
		 Avatar-only packets have no position until death, and 
		 are linked to a vehicle. After death they have no ID for a 
		 Vehicle anymore, and a position instead.
		 Before death only "Vehicle in ID 1" packets have a position.
		*/
		if (packet.EntityId2 != 0 && Replay!.Entities.ContainsKey(packet.EntityId1) && Replay!.Entities.ContainsKey(packet.EntityId2))
		{
			/*
				This serves to link the positions of the two entities 
				where the position of entity 1 is set by wherever entity 2
				is, rather than by the position field.
				e.g. Assigning the Avatar the position of the Vehicle
			 */
			var masterEntity = Replay!.Entities[packet.EntityId2];
			var slaveEntity = Replay!.Entities[packet.EntityId1];

			slaveEntity.SetPosition(slaveEntity.GetPosition());
		}
		else if (packet.EntityId1 != 0 && packet.EntityId2 == 0
			&& Replay!.Entities.ContainsKey(packet.EntityId1))
		{
			// This is a regular update for entity 1, without entity 2
			var entity = Replay!.Entities[packet.EntityId1];
			entity.SetPosition(packet.Position);
		}
	}

	public virtual void OnEntityMethod(EntityMethodPacket packet)
	{
		var entity = Replay!.Entities[packet.EntityId];
		using BinaryReader methodDataReader = packet.Data.GetBinaryReader();
		entity.CallClientMethod(packet.MessageId, methodDataReader, this);
	}
	public virtual void OnEntityProperty(EntityPropertyPacket packet)
	{
		var entity = Replay!.Entities[packet.EntityId];
		using BinaryReader propertyData = packet.Data.GetBinaryReader();
		entity.SetClientProperty(packet.MessageId, propertyData, this);
	}

	public virtual void OnNestedProperty(NestedPropertyPacket packet)
	{
		var entity = Replay!.Entities[(int)packet.EntityId];
		packet.Apply(entity);
	}

	protected virtual Entity CreateEntity(int id, string name)
		=> new(id, name, _definitionStore.GetEntityDefinitionByName(Replay!.ClientVersion, name), _methodSubscriptions, _propertyChangedSubscriptions, _entityLogger);

	protected virtual Entity CreateEntity(int id, int index)
	{
		var definition = _definitionStore.GetEntityDefinitionByIndex(Replay!.ClientVersion, index);
		return new(id, definition.Name, definition, _methodSubscriptions, _propertyChangedSubscriptions, _entityLogger);
	}
}
