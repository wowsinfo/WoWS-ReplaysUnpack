using Nodsoft.WowsReplaysUnpack.Core.Extensions;

namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

/// <summary>
/// Fires when servers requests client to change
/// entity's property with some arguments
/// </summary>
public class EntityPropertyPacket : NetworkPacketBase
{
	public uint EntityId { get; }
	public uint MessageId { get; }
	public byte[] Data { get; }

	public EntityPropertyPacket(int packetIndex, BinaryReader binaryReader) : base(packetIndex)
	{
		EntityId = binaryReader.ReadUInt32();
		MessageId = binaryReader.ReadUInt32();
		Data = binaryReader.ReadBytesWithHeader();
	}

}
