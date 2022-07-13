using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

namespace Nodsoft.WowsReplaysUnpack.Services;

public class DefaultReplayController : IReplayController
{
	public virtual UnpackedReplay? Replay { get; protected set; }

	public virtual UnpackedReplay CreateUnpackedReplay(ArenaInfo arenaInfo)
	{
		Replay = new UnpackedReplay(arenaInfo);
		return Replay!;
	}

	public virtual void HandleNetworkPacket(INetworkPacket networkPacket)
	{
		if (networkPacket is BasePlayerCreatePacket packet)
			OnBasePlayerCreate(packet);
	}

	public virtual void OnBasePlayerCreate(BasePlayerCreatePacket packet)
	{

	}
}
