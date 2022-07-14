namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

/// <summary>
/// Fires when entity leaves AOI and stops
/// receiving updates from server
/// </summary>
public class EntityLeavePacket : INetworkPacket
{
	public int EntityId { get; }

	public EntityLeavePacket(BinaryReader binaryReader)
	{
		EntityId = binaryReader.ReadInt32();
	}

}
