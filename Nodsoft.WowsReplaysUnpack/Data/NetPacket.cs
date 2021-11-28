using System;
using System.IO;


namespace Nodsoft.WowsReplaysUnpack.Data;


public struct NetPacket
{
	public uint Size { get; set; }
	public string Type { get; set; }
	public float Time { get; set; }
	public MemoryStream RawData { get; set; }


	public NetPacket(MemoryStream stream)
	{
		byte[] payloadSize = new byte[4];
		byte[] payloadType = new byte[4];
		byte[] payloadTime = new byte[4];

		stream.Read(payloadSize);
		stream.Read(payloadType);
		stream.Read(payloadTime);

		Size = BitConverter.ToUInt32(payloadSize);
		Type = BitConverter.ToUInt32(payloadType).ToString("X2");
		Time = BitConverter.ToSingle(payloadTime);

		byte[] data = new byte[Size];
		stream.Read(data);
		RawData = new MemoryStream(data);
	}
}
