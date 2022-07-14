using Nodsoft.WowsReplaysUnpack.Core.Extensions;

namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

/// <summary>
/// This packet is sent to create a new player as far as required to
/// talk to the base entity. Only data shared between the base and the
/// client is provided in this method - the cell data will be provided by the
/// CellPlayerCreatePacket later if the player is put on the cell also.
/// </summary>
public class BasePlayerCreatePacket : INetworkPacket
{
	public int EntityId { get; }
	public short EntityType { get; }
	public byte[] Data { get; }
	public BasePlayerCreatePacket(BinaryReader binaryReader)
	{
		EntityId = binaryReader.ReadInt32();
		EntityType = binaryReader.ReadInt16();
		Data = binaryReader.ReadBytesWithHeader();
	}
}
