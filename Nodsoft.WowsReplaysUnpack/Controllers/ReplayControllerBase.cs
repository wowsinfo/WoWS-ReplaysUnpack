using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Entities;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;
using Nodsoft.WowsReplaysUnpack.Core.Security;
using System.Reflection;

namespace Nodsoft.WowsReplaysUnpack.Controllers;

public abstract class ReplayControllerBase<T> : IReplayController
	where T : class, IReplayController
{
	private static readonly Dictionary<string, MethodInfo[]> _methodSubscriptions;
	private static readonly Dictionary<string, MethodInfo[]> _propertyChangedSubscriptions;
	protected IDefinitionStore DefinitionStore { get; }
	protected ILogger<Entity> EntityLogger { get; }

	public UnpackedReplay Replay { get; protected set; }

	static ReplayControllerBase()
	{
		_methodSubscriptions = typeof(T).GetMethods()
			.Select(m => new { Attribute = m.GetCustomAttribute<MethodSubscriptionAttribute>(), MethodInfo = m })
			.Where(m => m.Attribute is not null)
			.GroupBy(m => $"{m.Attribute!.EntityName}_{m.Attribute.MethodName}")
			.ToDictionary(
				m => m.Key, 
				m => m.OrderBy(m => m.Attribute?.Priority).Select(m => m.MethodInfo).ToArray());

		_propertyChangedSubscriptions = typeof(T).GetMethods()
			.Select(m => new { Attribute = m.GetCustomAttribute<PropertyChangedSubscriptionAttribute>(), MethodInfo = m })
			.Where(m => m.Attribute is not null)
			.GroupBy(m => $"{m.Attribute!.EntityName}_{m.Attribute.PropertyName}")
			.ToDictionary(
				m => m.Key, 
				m => m.Select(m => m.MethodInfo).ToArray());
	}

#pragma warning disable CS8618 // Replay Property is never null because after creating the controller, CreateUnpackedReplay is called
	
	protected ReplayControllerBase(IDefinitionStore definitionStore,

		// ReSharper disable once ContextualLoggerProblem
		ILogger<Entity> entityLogger)
	{
		DefinitionStore = definitionStore;
		EntityLogger = entityLogger;
	}
	
#pragma warning restore CS8618
	
	
	public virtual UnpackedReplay CreateUnpackedReplay(ArenaInfo arenaInfo)
	{
		Replay = new(arenaInfo);
		return Replay;
	}

	#region Packet Handling
	public virtual void HandleNetworkPacket(NetworkPacketBase networkPacket, ReplayUnpackerOptions options)
	{
		Action? action = networkPacket switch
		{
			MapPacket p => () => OnMap(p),
			BasePlayerCreatePacket p => () => OnBasePlayerCreate(p),
			CellPlayerCreatePacket p => () => OnCellPlayerCreate(p),
			EntityEnterPacket p when Replay.Entities.ContainsKey(p.EntityId) => () => Replay.Entities[p.EntityId].IsInAoI = true,
			EntityLeavePacket p when Replay.Entities.ContainsKey(p.EntityId) => () => Replay.Entities[p.EntityId].IsInAoI = false,
			EntityCreatePacket p => () => OnEntityCreate(p),
			PositionPacket p => () => OnPosition(p),
			PlayerPositionPacket p => () => OnPlayerPosition(p),
			EntityMethodPacket p => () => OnEntityMethod(p),
			EntityPropertyPacket p => () => OnEntityProperty(p),
			NestedPropertyPacket p => () => OnNestedProperty(p),
			_ => null
		};

		action?.Invoke();
	}

	protected virtual void OnMap(MapPacket packet) => Replay.MapName = packet.Name;

	protected virtual void OnBasePlayerCreate(BasePlayerCreatePacket packet)
	{
		Replay.Entities.GetOrAddValue(packet.EntityId, out Entity? entity, () => CreateEntity(packet.EntityId, "Avatar"));

		using BinaryReader binaryReader = packet.Data.GetBinaryReader();
		entity.SetBaseProperties(binaryReader);
		Replay.PlayerEntityId = packet.EntityId;
	}

	protected virtual void OnCellPlayerCreate(CellPlayerCreatePacket packet)
	{
		Replay.Entities.GetOrAddValue(packet.EntityId, out Entity? entity, () => CreateEntity(packet.EntityId, "Avatar"));

		using BinaryReader binaryReader = packet.Data.GetBinaryReader();
		entity.SetInternalClientProperties(binaryReader);
	}

	protected virtual void OnEntityCreate(EntityCreatePacket packet)
	{
		Entity entity = CreateEntity(packet.EntityId, packet.Type);

		Replay.Entities[packet.EntityId] = entity;
		using BinaryReader binaryReader = packet.Data.GetBinaryReader();
		byte valuesCount = binaryReader.ReadByte();

		for (int i = 0; i < valuesCount; i++)
		{
			byte propertyIndex = binaryReader.ReadByte();
			entity.SetClientProperty(propertyIndex, binaryReader, this);
		}
	}

	protected virtual void OnPosition(PositionPacket packet)
	{
		if (!Replay.Entities.ContainsKey(packet.EntityId))
		{
			return;
		}

		Entity entity = Replay.Entities[packet.EntityId];
		entity.SetPosition(packet.Position);
	}

	protected virtual void OnPlayerPosition(PlayerPositionPacket packet)
	{
		/* 
		 Entity at ID 1 is the primary one's position being updated
		 Avatar-only packets have no position until death, and 
		 are linked to a vehicle. After death they have no ID for a 
		 Vehicle anymore, and a position instead.
		 Before death only "Vehicle in ID 1" packets have a position.
		*/
		if (packet.EntityId2 is not 0 && Replay.Entities.ContainsKey(packet.EntityId1) && Replay.Entities.ContainsKey(packet.EntityId2))
		{
			/*
				This serves to link the positions of the two entities 
				where the position of entity 1 is set by wherever entity 2
				is, rather than by the position field.
				e.g. Assigning the Avatar the position of the Vehicle
			 */
			Entity masterEntity = Replay.Entities[packet.EntityId2];
			Entity slaveEntity = Replay.Entities[packet.EntityId1];

			slaveEntity.SetPosition(masterEntity.GetPosition());
		}
		else if (packet is { EntityId1: not 0, EntityId2: 0 } && Replay.Entities.ContainsKey(packet.EntityId1))
		{
			// This is a regular update for entity 1, without entity 2
			Entity entity = Replay.Entities[packet.EntityId1];
			entity.SetPosition(packet.Position);
		}
	}

	protected virtual void OnEntityMethod(EntityMethodPacket packet)
	{
		if (!Replay.Entities.ContainsKey(packet.EntityId))
		{
			return;
		}

		Entity entity = Replay.Entities[packet.EntityId];
		using BinaryReader methodDataReader = packet.Data.GetBinaryReader();
		entity.CallClientMethod(packet.MessageId, packet.PacketTime, methodDataReader, this);
	}

	protected virtual void OnEntityProperty(EntityPropertyPacket packet)
	{
		if (!Replay.Entities.ContainsKey(packet.EntityId))
		{
			return;
		}

		Entity entity = Replay.Entities[packet.EntityId];
		using BinaryReader propertyData = packet.Data.GetBinaryReader();
		entity.SetClientProperty(packet.MessageId, propertyData, this);
	}

	protected virtual void OnNestedProperty(NestedPropertyPacket packet)
	{
		if (!Replay.Entities.ContainsKey(packet.EntityId))
		{
			return;
		}

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

	[MethodSubscription("Avatar", "onArenaStateReceived", ParamsAsDictionary = true, Priority = -1)]
	public void OnArenaStateReceivedCVECheck(Dictionary<string, object?> arguments)
	{
		CveChecks.ScanForCVE_2022_31265((byte[])arguments["preBattlesInfo"]!, "Avatar_onArenaStateReceived_preBattlesInfo");
		CveChecks.ScanForCVE_2022_31265((byte[])arguments["playersStates"]!, "Avatar_onArenaStateReceived_playersStates");
		CveChecks.ScanForCVE_2022_31265((byte[])arguments["observersState"]!, "Avatar_onArenaStateReceived_observersState");
		CveChecks.ScanForCVE_2022_31265((byte[])arguments["buildingsInfo"]!, "Avatar_onArenaStateReceived_buildingsInfo");
	}
	#endregion
}
