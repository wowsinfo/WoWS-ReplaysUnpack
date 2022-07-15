using Nodsoft.WowsReplaysUnpack.Core.Extensions;

namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

public class CellPlayerCreatePacket : ANetworkPacket
{
	public uint EntityId { get; }
	public byte[] Data { get; }
	public CellPlayerCreatePacket(int packetIndex, BinaryReader binaryReader) : base(packetIndex)
	{
		EntityId = (uint)binaryReader.ReadInt32();
		_ = binaryReader.ReadInt32(); // spaceId;
		_ = binaryReader.ReadInt32(); // vehicleId;
		_ = binaryReader.ReadVector3(); // position (Vector3, 3xfloat)
		_ = binaryReader.ReadVector3(); // direction (Vector3, 3xfloat)
		Data = binaryReader.ReadBytesWithHeader();
	}
}
