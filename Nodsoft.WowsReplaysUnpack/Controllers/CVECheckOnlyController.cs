using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Entities;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

namespace Nodsoft.WowsReplaysUnpack.Controllers;

public class CVECheckOnlyController : AReplayControllerBase<CVECheckOnlyController>
{
	public CVECheckOnlyController(IDefinitionStore definitionStore, ILogger<Entity> entityLogger) : base(definitionStore, entityLogger)
	{
	}

	public override void HandleNetworkPacket(ANetworkPacket networkPacket, ReplayUnpackerOptions options)
	{
		if (networkPacket is BasePlayerCreatePacket bpPacker)
			OnBasePlayerCreate(bpPacker);
		if (networkPacket is CellPlayerCreatePacket cpPacket)
			OnCellPlayerCreate(cpPacket);
		if (networkPacket is EntityMethodPacket entityMethod)
			OnEntityMethod(entityMethod);
	}

	public override void OnEntityMethod(EntityMethodPacket packet)
	{
		if (!Replay.Entities.ContainsKey(packet.EntityId))
			return;

		Entity entity = Replay.Entities[packet.EntityId];
		if (entity.Name != "Avatar" && entity.GetClientMethodNameForIndex(packet.MessageId) != "onArenaStateReceived")
			return;
		using BinaryReader methodDataReader = packet.Data.GetBinaryReader();
		entity.CallClientMethod(packet.MessageId, packet.PacketTime, methodDataReader, this);
	}
}
