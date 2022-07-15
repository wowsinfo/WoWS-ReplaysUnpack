using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Numerics;

namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

public struct PositionContainer
{
	public PositionContainer(Vector3 position, float yaw, float pitch, float roll)
	{
		Position = position;
		Yaw = yaw;
		Pitch = pitch;
		Roll = roll;
	}

	public Vector3 Position { get; }
	public float Yaw { get; }
	public float Pitch { get; }
	public float Roll { get; }
}

public class PositionPacket : ANetworkPacket
{
	public int EntityId { get; }
	public int VehicleId { get; }
	public PositionContainer Position { get; }
	public Vector3 PositionError { get; }
	public bool IsError { get; }

	public PositionPacket(int packetIndex, BinaryReader binaryReader) : base(packetIndex)
	{
		EntityId = binaryReader.ReadInt32();
		VehicleId = binaryReader.ReadInt32();
		Vector3 position = binaryReader.ReadVector3();
		PositionError = binaryReader.ReadVector3();
		float yaw = binaryReader.ReadSingle();
		float pitch = binaryReader.ReadSingle();
		float roll = binaryReader.ReadSingle();
		IsError = binaryReader.ReadBoolean();

		Position = new(position, yaw, pitch, roll);
	}

}
