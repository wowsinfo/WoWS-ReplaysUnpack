using Nodsoft.WowsReplaysUnpack.Core.Extensions;

namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

/// <summary>
/// Fires when servers requests client to change
/// entity's property with some arguments
/// </summary>
public class EntityPropertyPacket : ANetworkPacket
{
	public int EntityId { get; }
	public int MessageId { get; }
	public byte[] Data { get; }

	public EntityPropertyPacket(int packetIndex, BinaryReader binaryReader) : base(packetIndex)
	{
		EntityId = binaryReader.ReadInt32();
		MessageId = binaryReader.ReadInt32();
		Data = binaryReader.ReadBytesWithHeader();
	}

}
