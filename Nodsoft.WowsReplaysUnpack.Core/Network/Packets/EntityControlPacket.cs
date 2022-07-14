namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

public class EntityControlPacket : INetworkPacket
{
	public int EntityId { get; }
	public bool IsControlled { get; }

	public EntityControlPacket(BinaryReader binaryReader)
	{
		EntityId = binaryReader.ReadInt32();
		IsControlled = binaryReader.ReadBoolean();
	}

}
