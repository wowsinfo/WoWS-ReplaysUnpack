namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

/// <summary>
/// Fires when entity enters AOI and starts
/// receiving updates from server
/// </summary>
public class EntityEnterPacket : ANetworkPacket
{
	public uint EntityId { get; }
	public int SpaceId { get; }
	public int VehicleId { get; }
	public EntityEnterPacket(int packetIndex, BinaryReader binaryReader) : base(packetIndex)
	{
		EntityId = (uint)binaryReader.ReadInt32();
		SpaceId = binaryReader.ReadInt32();
		VehicleId = binaryReader.ReadInt32();
	}
}
