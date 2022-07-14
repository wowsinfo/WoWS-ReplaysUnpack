using Nodsoft.WowsReplaysUnpack.Core.Extensions;

namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

/// <summary>
/// Fires when servers requests client to call
/// entity's method with some arguments
/// </summary>
public class EntityMethodPacket : INetworkPacket
{
	public int EntityId { get; }
	public int MessageId { get; }
	public byte[] Data { get; }

	public EntityMethodPacket(BinaryReader binaryReader)
	{
		EntityId = binaryReader.ReadInt32();
		MessageId = binaryReader.ReadInt32();
		Data = binaryReader.ReadBytesWithHeader();
	}

}
