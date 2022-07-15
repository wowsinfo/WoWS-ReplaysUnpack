namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

public abstract class ANetworkPacket
{
	public int PacketIndex { get; }
	public ANetworkPacket(int packetIndex)
	{
		PacketIndex = packetIndex;
	}
}
