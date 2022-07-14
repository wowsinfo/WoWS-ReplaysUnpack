using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Numerics;

namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

public class PositionPacket : INetworkPacket
{
	public int EntityId { get; }
	public int VehicleId { get; }
	public Vector3 Position { get; }
	public Vector3 PositionError { get; }
	public float Yaw { get; }
	public float Pitch { get; }
	public float Roll { get; }
	public bool IsError { get; }

	public PositionPacket(BinaryReader binaryReader)
	{
		EntityId = binaryReader.ReadInt32();
		VehicleId = binaryReader.ReadInt32();
		Position = binaryReader.ReadVector3();
		PositionError = binaryReader.ReadVector3();
		Yaw = binaryReader.ReadSingle();
		Pitch = binaryReader.ReadSingle();
		Roll = binaryReader.ReadSingle();
		IsError = binaryReader.ReadBoolean();
	}

}
