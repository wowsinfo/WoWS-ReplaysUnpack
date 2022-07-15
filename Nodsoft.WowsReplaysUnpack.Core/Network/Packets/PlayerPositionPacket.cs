using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Numerics;

namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

public class PlayerPositionPacket : ANetworkPacket
{
	public uint EntityId1 { get; } = 0;
	public uint EntityId2 { get; } = 0;
	public PositionContainer Position { get; }

	public PlayerPositionPacket(int packetIndex, BinaryReader binaryReader) : base(packetIndex)
	{
		EntityId1 = (uint)binaryReader.ReadInt32();
		EntityId2 = (uint)binaryReader.ReadInt32();

		Vector3 position = binaryReader.ReadVector3();
		float yaw = binaryReader.ReadSingle();
		float pitch = binaryReader.ReadSingle();
		float roll = binaryReader.ReadSingle();

		Position = new(position, yaw, pitch, roll);
	}
}
