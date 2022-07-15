using Nodsoft.WowsReplaysUnpack.Core.Extensions;

namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

public class EntityCreatePacket : ANetworkPacket
{
	public int EntityId { get; }
	public short Type { get; }
	public int VehicleId { get; }
	public int SpaceId { get; }
	public byte[] Data { get; }

	public EntityCreatePacket(int packetIndex, BinaryReader binaryReader) : base(packetIndex)
	{
		EntityId = binaryReader.ReadInt32();
		Type = binaryReader.ReadInt16();
		VehicleId = binaryReader.ReadInt32();
		SpaceId = binaryReader.ReadInt32();
		_ = binaryReader.ReadVector3(); // position
		_ = binaryReader.ReadVector3(); // direction
		Data = binaryReader.ReadBytesWithHeader();
	}
}
