namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

/// <summary>
/// Fires when entity enters AOI and starts
/// receiving updates from server
/// </summary>
public class EntityEnterPacket : INetworkPacket
{
	public int EntityId { get; }
	public int SpaceId { get; }
	public int VehicleId { get; }
	public EntityEnterPacket(BinaryReader binaryReader)
	{
		EntityId = binaryReader.ReadInt32();
		SpaceId = binaryReader.ReadInt32();
		VehicleId = binaryReader.ReadInt32();
	}
}
