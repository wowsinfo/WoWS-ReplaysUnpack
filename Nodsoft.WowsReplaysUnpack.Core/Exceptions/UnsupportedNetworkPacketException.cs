namespace Nodsoft.WowsReplaysUnpack.Core.Exceptions;

public class UnsupportedNetworkPacketException : Exception
{
  public uint PacketType { get; }
	public UnsupportedNetworkPacketException(uint packetType)
	{
		PacketType = packetType;
	}
}
