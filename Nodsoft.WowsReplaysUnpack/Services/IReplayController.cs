using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;
using Nodsoft.WowsReplaysUnpack.Models.Replay;

namespace Nodsoft.WowsReplaysUnpack.Services;

public interface IReplayController
{
	void HandleNetworkPacket(UnpackedReplay replay, INetworkPacket networkPacket);
}
