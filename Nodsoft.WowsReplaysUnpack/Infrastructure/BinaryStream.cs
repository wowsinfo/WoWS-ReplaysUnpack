using System;
using System.IO;


namespace Nodsoft.WowsReplaysUnpack.Infrastructure;

internal class BinaryStream
{
	public MemoryStream Value { get; init; }

	public uint Length { get; init; }

	public BinaryStream(MemoryStream stream)
	{
		//stream.Seek(0, SeekOrigin.Begin);
		byte[] bLength = new byte[4];
		stream.Read(bLength);
		Length = BitConverter.ToUInt32(bLength);
		byte[] bValue = new byte[Length];
		stream.Read(bValue);
		Value = new MemoryStream(bValue);
	}


}
