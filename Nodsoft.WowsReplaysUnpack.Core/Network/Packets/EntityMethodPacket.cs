using Nodsoft.WowsReplaysUnpack.Core.Extensions;

namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

/// <summary>
/// Fires when servers requests client to call
/// entity's method with some arguments
/// </summary>
public class EntityMethodPacket : ANetworkPacket
{
	public uint EntityId { get; }
	public uint MessageId { get; }
	public byte[] Data { get; }

	public EntityMethodPacket(int packetIndex, BinaryReader binaryReader) : base(packetIndex)
	{
		EntityId = binaryReader.ReadUInt32();
		MessageId = binaryReader.ReadUInt32();
		Data = binaryReader.ReadBytesWithHeader();
	}

}
