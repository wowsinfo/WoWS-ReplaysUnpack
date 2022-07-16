using Nodsoft.WowsReplaysUnpack.Core.Extensions;

namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

/// <summary>
/// Fires when servers requests client to call
/// entity's method with some arguments
/// </summary>
public class EntityMethodPacket : NetworkPacketBase
{
	public uint EntityId { get; }
	public uint MessageId { get; }
	public byte[] Data { get; }
	public float PacketTime { get; } // Time in seconds from battle start

	public EntityMethodPacket(int packetIndex, float packetTime, BinaryReader binaryReader) : base(packetIndex)
	{
		EntityId = binaryReader.ReadUInt32();
		MessageId = binaryReader.ReadUInt32();
		Data = binaryReader.ReadBytesWithHeader();
		PacketTime = packetTime;
	}
}
