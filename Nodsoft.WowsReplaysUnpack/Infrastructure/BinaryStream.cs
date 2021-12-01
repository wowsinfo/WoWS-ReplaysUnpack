using System;
using System.IO;


namespace Nodsoft.WowsReplaysUnpack.Infrastructure;

internal sealed class BinaryStream
{
	public MemoryStream Value { get; }

	public uint Length { get; }

	public BinaryStream(Stream stream)
	{
		byte[] bLength = new byte[4];
		stream.Read(bLength);
		Length = BitConverter.ToUInt32(bLength);
		byte[] bValue = new byte[Length];
		stream.Read(bValue);
		Value = new(bValue);
	}
}
