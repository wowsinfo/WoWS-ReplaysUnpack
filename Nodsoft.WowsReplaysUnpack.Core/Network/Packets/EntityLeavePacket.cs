namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

/// <summary>
/// Fires when entity leaves AOI and stops
/// receiving updates from server
/// </summary>
public class EntityLeavePacket : ANetworkPacket
{
	public int EntityId { get; }

	public EntityLeavePacket(int packetIndex, BinaryReader binaryReader) : base(packetIndex)
	{
		EntityId = binaryReader.ReadInt32();
	}

}
