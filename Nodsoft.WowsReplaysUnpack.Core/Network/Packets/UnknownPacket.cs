namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

public class UnknownPacket : NetworkPacketBase
{
	public byte[] Data { get; set; }
	public UnknownPacket(int packetIndex, BinaryReader binaryReader) : base(packetIndex) 
		=> Data = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length - (int)binaryReader.BaseStream.Position);
}
