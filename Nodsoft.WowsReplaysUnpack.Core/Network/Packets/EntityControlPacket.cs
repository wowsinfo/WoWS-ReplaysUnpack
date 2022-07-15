namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

public class EntityControlPacket : ANetworkPacket
{
	public int EntityId { get; }
	public bool IsControlled { get; }

	public EntityControlPacket(int packetIndex, BinaryReader binaryReader) : base(packetIndex)
	{
		EntityId = binaryReader.ReadInt32();
		IsControlled = binaryReader.ReadBoolean();
	}

}
