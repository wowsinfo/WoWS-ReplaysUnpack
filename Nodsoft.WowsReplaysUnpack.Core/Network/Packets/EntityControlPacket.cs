namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

public class EntityControlPacket : NetworkPacketBase
{
	public uint EntityId { get; }
	public bool IsControlled { get; }

	public EntityControlPacket(int packetIndex, BinaryReader binaryReader) : base(packetIndex)
	{
		EntityId = (uint)binaryReader.ReadInt32();
		IsControlled = binaryReader.ReadBoolean();
	}

}
