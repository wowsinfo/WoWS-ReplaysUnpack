using System;
using System.IO;


namespace Nodsoft.WowsReplaysUnpack.Data;


internal readonly struct ReplayBlob
{
	public byte[] Data { get; }

	public ReplayBlob(Stream stream)
	{
		byte[] bSize = new byte[1];
		stream.Read(bSize);

		if (bSize[0] is 255)
		{
			byte[] bRealSize = new byte[2];
			stream.Read(bRealSize);
			stream.Read(new byte[1]); // padding
			Data = new byte[BitConverter.ToUInt16(bRealSize)];
			stream.Read(Data);
		}
		else
		{
			Data = new byte[bSize[0]];
			stream.Read(Data);
		}
	}

	/*
	 * Interpreted from Python code
	 *

		size, = unpack('B', stream.read(1))
		# hack for arenaStateReceived
		if size == 0xff:
			size, = unpack('H', stream.read(2))
			# some dummy shit
			unpack('B', stream.read(1))
			return stream.read(size)
		return stream.read(size)
	*/
}
