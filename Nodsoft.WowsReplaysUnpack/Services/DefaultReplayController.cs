using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;
using Nodsoft.WowsReplaysUnpack.Models.Replay;

namespace Nodsoft.WowsReplaysUnpack.Services;

public class DefaultReplayController : IReplayController
{
	public virtual void HandleNetworkPacket(UnpackedReplay replay, INetworkPacket networkPacket)
	{
		if (networkPacket is BasePlayerCreatePacket packet)
			OnBasePlayerCreate(replay, packet);
	}

	public virtual void OnBasePlayerCreate(UnpackedReplay replay, BasePlayerCreatePacket packet)
	{

	}
}
