using System;
using System.IO;

namespace Nodsoft.WowsReplaysUnpack.Data.Raw;


internal readonly struct NetPacket
{
	public uint Size { get; }
	public uint Type { get; }
	public float Time { get; }
	public MemoryStream RawData { get; }

	public NetPacket(Stream stream)
	{
		byte[] payloadSize = new byte[4];
		byte[] payloadType = new byte[4];
		byte[] payloadTime = new byte[4];

		stream.Read(payloadSize);
		stream.Read(payloadType);
		stream.Read(payloadTime);

		Size = BitConverter.ToUInt32(payloadSize);
		Type = BitConverter.ToUInt32(payloadType);
		Time = BitConverter.ToSingle(payloadTime);

		byte[] data = new byte[Size];
		stream.Read(data);
		RawData = new(data);
	}
}
