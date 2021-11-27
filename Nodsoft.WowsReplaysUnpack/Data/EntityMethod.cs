using Nodsoft.WowsReplaysUnpack.Infrastructure;
using System;
using System.IO;


namespace Nodsoft.WowsReplaysUnpack.Data;


internal sealed record EntityMethod
{
	public uint EntityId { get; set; }
	public uint MessageId { get; set; }
	public BinaryStream Data { get; set; }

	public EntityMethod(MemoryStream stream)
	{
		byte[] bEntityId = new byte[4];
		byte[] bMessageId = new byte[4];

		stream.Read(bEntityId);
		stream.Read(bMessageId);

		EntityId = BitConverter.ToUInt32(bEntityId);
		MessageId = BitConverter.ToUInt32(bMessageId);

		Data = new BinaryStream(stream);
	}
}
