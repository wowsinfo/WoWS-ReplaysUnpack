using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

namespace Nodsoft.WowsReplaysUnpack.Controllers;

public interface IReplayController
{
	UnpackedReplay CreateUnpackedReplay(ArenaInfo arenaInfo);
	void HandleNetworkPacket(ANetworkPacket networkPacket, ReplayUnpackerOptions options);
}
