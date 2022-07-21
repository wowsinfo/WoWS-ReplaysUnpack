using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Entities;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

namespace Nodsoft.WowsReplaysUnpack.Controllers;

public class CveCheckOnlyController : ReplayControllerBase<CveCheckOnlyController>
{
	public CveCheckOnlyController(IDefinitionStore definitionStore, ILogger<Entity> entityLogger) : base(definitionStore, entityLogger)
	{
	}

	public override void HandleNetworkPacket(NetworkPacketBase networkPacket, ReplayUnpackerOptions options)
	{
		if (networkPacket is BasePlayerCreatePacket bpPacker)
		{
			OnBasePlayerCreate(bpPacker);
		}

		if (networkPacket is CellPlayerCreatePacket cpPacket)
		{
			OnCellPlayerCreate(cpPacket);
		}

		if (networkPacket is EntityMethodPacket entityMethod)
		{
			OnEntityMethod(entityMethod);
		}
	}

	protected override void OnEntityMethod(EntityMethodPacket packet)
	{
		if (!Replay.Entities.ContainsKey(packet.EntityId))
		{
			return;
		}

		Entity entity = Replay.Entities[packet.EntityId];
		if (entity.Name is not "Avatar" && entity.GetClientMethodNameForIndex(packet.MessageId) is not "onArenaStateReceived")
		{
			return;
		}

		using BinaryReader methodDataReader = packet.Data.GetBinaryReader();
		entity.CallClientMethod(packet.MessageId, packet.PacketTime, methodDataReader, this);
	}
}
