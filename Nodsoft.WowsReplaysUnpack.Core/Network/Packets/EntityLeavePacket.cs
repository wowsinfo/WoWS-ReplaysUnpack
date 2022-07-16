namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

/// <summary>
/// Fires when entity leaves AOI and stops
/// receiving updates from server
/// </summary>
public class EntityLeavePacket : NetworkPacketBase
{
	public uint EntityId { get; }

	public EntityLeavePacket(int packetIndex, BinaryReader binaryReader) : base(packetIndex) => EntityId = (uint)binaryReader.ReadInt32();
}