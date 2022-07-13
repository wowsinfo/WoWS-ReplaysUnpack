using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

namespace Nodsoft.WowsReplaysUnpack.Services;

public interface IReplayController
{
	UnpackedReplay CreateUnpackedReplay(ArenaInfo arenaInfo);
	void HandleNetworkPacket(INetworkPacket networkPacket);
}
