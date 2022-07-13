namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

public class UnknownPacket : INetworkPacket
{
	public byte[] Data { get; set; }
	public UnknownPacket(BinaryReader binaryReader)
	{
		Data = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length - (int)binaryReader.BaseStream.Position);
	}
}
