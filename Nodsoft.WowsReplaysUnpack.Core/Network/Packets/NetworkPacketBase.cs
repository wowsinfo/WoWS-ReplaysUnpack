namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

public abstract class NetworkPacketBase
{
	public int PacketIndex { get; }
	protected NetworkPacketBase(int packetIndex) => PacketIndex = packetIndex;
}
