using Nodsoft.WowsReplaysUnpack.Core.Extensions;

namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

internal class EntityCreatePacket : INetworkPacket
{
	public int EntityId { get; }
	public short Type { get; }
	public int VehicleId { get; }
	public int SpaceId { get; }
	public byte[] Data { get; }

	public EntityCreatePacket(BinaryReader binaryReader)
	{
		EntityId = binaryReader.ReadInt32();
		Type = binaryReader.ReadInt16();
		VehicleId = binaryReader.ReadInt32();
		SpaceId = binaryReader.ReadInt32();
		_ = binaryReader.ReadVector3(); // position
		_ = binaryReader.ReadVector3(); // direction
		_ = binaryReader.ReadInt32(); // unknown
		Data = binaryReader.ReadBytesWithHeader();
	}
}
