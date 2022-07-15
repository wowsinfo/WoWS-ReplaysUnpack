using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Entities;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;
using Nodsoft.WowsReplaysUnpack.Core.Security;
using System.Reflection;

namespace Nodsoft.WowsReplaysUnpack.Controllers;

public abstract class AReplayControllerBase<T> : IReplayController
	where T : class, IReplayController
{
	private static readonly Dictionary<string, MethodInfo[]> _methodSubscriptions;
	private static readonly Dictionary<string, MethodInfo[]> _propertyChangedSubscriptions;
	protected IDefinitionStore DefinitionStore { get; }
	protected ILogger<Entity> EntityLogger { get; }

	public UnpackedReplay Replay { get; protected set; }

	static AReplayControllerBase()
	{
		_methodSubscriptions = typeof(T).GetMethods()
			.Select(m => new { Attribute = m.GetCustomAttribute<MethodSubscriptionAttribute>(), MethodInfo = m })
			.Where(m => m.Attribute is not null)
			.GroupBy(m => $"{m.Attribute!.EntityName}_{m.Attribute.MethodName}")
			.ToDictionary(m => m.Key, m => m.Select(m => m.MethodInfo).ToArray());

		_propertyChangedSubscriptions = typeof(T).GetMethods()
			.Select(m => new { Attribute = m.GetCustomAttribute<PropertyChangedSubscriptionAttribute>(), MethodInfo = m })
			.Where(m => m.Attribute is not null)
			.GroupBy(m => $"{m.Attribute!.EntityName}_{m.Attribute.PropertyName}")
			.ToDictionary(m => m.Key, m => m.Select(m => m.MethodInfo).ToArray());
	}

#pragma warning disable CS8618 // Replay Property is never null because after creating the controller, CreateUnpackedReplay is called
	public AReplayControllerBase(IDefinitionStore definitionStore,
#pragma warning restore CS8618
		ILogger<Entity> entityLogger)
	{
		DefinitionStore = definitionStore;
		EntityLogger = entityLogger;
	}

	public virtual UnpackedReplay CreateUnpackedReplay(ArenaInfo arenaInfo)
	{
		Replay = new UnpackedReplay(arenaInfo);
		return Replay;
	}

	#region Packet Handling
	public virtual void HandleNetworkPacket(ANetworkPacket networkPacket, ReplayUnpackerOptions options)
	{
		if (networkPacket is MapPacket mapPacket)
			OnMap(mapPacket);
		else if (networkPacket is BasePlayerCreatePacket _1)
			OnBasePlayerCreate(_1);
		else if (networkPacket is CellPlayerCreatePacket _2)
			OnCellPlayerCreate(_2);
		else if (networkPacket is EntityEnterPacket _3 && Replay.Entities.ContainsKey(_3.EntityId))
			Replay.Entities[_3.EntityId].IsInAoI = true;
		else if (networkPacket is EntityLeavePacket _4 && Replay.Entities.ContainsKey(_4.EntityId))
			Replay.Entities[_4.EntityId].IsInAoI = false;
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
		Replay.MapName = packet.Name;
	}

	public virtual void OnBasePlayerCreate(BasePlayerCreatePacket packet)
	{
		Replay.Entities.GetOrAddValue(packet.EntityId, out Entity? entity, () => CreateEntity(packet.EntityId, "Avatar"));

		using BinaryReader binaryReader = packet.Data.GetBinaryReader();
		entity.SetBaseProperties(binaryReader);
		Replay.PlayerEntityId = packet.EntityId;
	}

	public virtual void OnCellPlayerCreate(CellPlayerCreatePacket packet)
	{
		Replay.Entities.GetOrAddValue(packet.EntityId, out Entity? entity, () => CreateEntity(packet.EntityId, "Avatar"));

		using BinaryReader binaryReader = packet.Data.GetBinaryReader();
		entity.SetInternalClientProperties(binaryReader);
	}

	public virtual void OnEntityCreate(EntityCreatePacket packet)
	{
		Entity entity = CreateEntity(packet.EntityId, packet.Type);

		Replay.Entities[packet.EntityId] = entity;
		using BinaryReader binaryReader = packet.Data.GetBinaryReader();
		byte valuesCount = binaryReader.ReadByte();
		foreach (int i in Enumerable.Range(0, valuesCount))
		{
			byte propertyIndex = binaryReader.ReadByte();
			entity.SetClientProperty(propertyIndex, binaryReader, this);
		}
	}

	public virtual void OnPosition(PositionPacket packet)
	{
		if (!Replay.Entities.ContainsKey(packet.EntityId))
			return;
		Entity entity = Replay.Entities[packet.EntityId];
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
		if (packet.EntityId2 != 0 && Replay.Entities.ContainsKey(packet.EntityId1) && Replay.Entities.ContainsKey(packet.EntityId2))
		{
			/*
				This serves to link the positions of the two entities 
				where the position of entity 1 is set by wherever entity 2
				is, rather than by the position field.
				e.g. Assigning the Avatar the position of the Vehicle
			 */
			Entity? masterEntity = Replay.Entities[packet.EntityId2];
			Entity? slaveEntity = Replay.Entities[packet.EntityId1];

			slaveEntity.SetPosition(slaveEntity.GetPosition());
		}
		else if (packet.EntityId1 != 0 && packet.EntityId2 == 0
			&& Replay.Entities.ContainsKey(packet.EntityId1))
		{
			// This is a regular update for entity 1, without entity 2
			Entity? entity = Replay.Entities[packet.EntityId1];
			entity.SetPosition(packet.Position);
		}
	}

	public virtual void OnEntityMethod(EntityMethodPacket packet)
	{
		if (!Replay.Entities.ContainsKey(packet.EntityId))
			return;

		Entity entity = Replay.Entities[packet.EntityId];
		using BinaryReader methodDataReader = packet.Data.GetBinaryReader();
		entity.CallClientMethod(packet.MessageId, packet.PacketTime, methodDataReader, this);
	}

	public virtual void OnEntityProperty(EntityPropertyPacket packet)
	{
		if (!Replay.Entities.ContainsKey(packet.EntityId))
			return;

		Entity entity = Replay.Entities[packet.EntityId];
		using BinaryReader propertyData = packet.Data.GetBinaryReader();
		entity.SetClientProperty(packet.MessageId, propertyData, this);
	}

	public virtual void OnNestedProperty(NestedPropertyPacket packet)
	{
		if (!Replay.Entities.ContainsKey(packet.EntityId))
			return;

		Entity entity = Replay.Entities[packet.EntityId];
		packet.Apply(entity);
	}

	protected virtual Entity CreateEntity(uint id, string name)
		=> new(id, name, DefinitionStore.GetEntityDefinitionByName(Replay.ClientVersion, name), _methodSubscriptions, _propertyChangedSubscriptions, EntityLogger);

	protected virtual Entity CreateEntity(uint id, int index)
	{
		EntityDefinition definition = DefinitionStore.GetEntityDefinitionByIndex(Replay.ClientVersion, index - 1);
		return new(id, definition.Name, definition, _methodSubscriptions, _propertyChangedSubscriptions, EntityLogger);
	}

	#endregion

	#region Subscriptions

	[MethodSubscription("Avatar", "onArenaStateReceived", true)]
	public void OnArenaStateReceivedCVECheck(Entity entity, Dictionary<string, object?> arguments)
	{
		CVEChecks.ScanForCVE_2022_31265((byte[])arguments["preBattlesInfo"]!, "Avatar_onArenaStateReceived_preBattlesInfo");
		CVEChecks.ScanForCVE_2022_31265((byte[])arguments["playersStates"]!, "Avatar_onArenaStateReceived_playersStates");
		CVEChecks.ScanForCVE_2022_31265((byte[])arguments["observersState"]!, "Avatar_onArenaStateReceived_observersState");
		CVEChecks.ScanForCVE_2022_31265((byte[])arguments["buildingsInfo"]!, "Avatar_onArenaStateReceived_buildingsInfo");
	}
	#endregion
}
