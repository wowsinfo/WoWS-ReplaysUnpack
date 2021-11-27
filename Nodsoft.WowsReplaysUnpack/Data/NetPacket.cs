using System;
using System.IO;


namespace Nodsoft.WowsReplaysUnpack.Data;


public struct NetPacket
{
	public uint Size;
	public string Type;
	public float Time;
	public MemoryStream rawData;


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
		rawData = new MemoryStream(data);
	}
}
