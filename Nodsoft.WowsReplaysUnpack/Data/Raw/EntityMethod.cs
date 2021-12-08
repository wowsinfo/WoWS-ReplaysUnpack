using System;
using System.IO;

namespace Nodsoft.WowsReplaysUnpack.Data.Raw;


public sealed record EntityMethod
{
	public uint EntityId { get; }
	public uint MessageId { get; }
	public BinaryStream Data { get; }

	public EntityMethod(Stream stream)
	{
		byte[] bEntityId = new byte[4];
		byte[] bMessageId = new byte[4];

		stream.Read(bEntityId);
		stream.Read(bMessageId);

		EntityId = BitConverter.ToUInt32(bEntityId);
		MessageId = BitConverter.ToUInt32(bMessageId);

		Data = new(stream);
	}
}
