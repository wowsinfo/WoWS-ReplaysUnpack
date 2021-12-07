using Mapster;
using Nodsoft.WowsReplaysUnpack.Data;
using Nodsoft.WowsReplaysUnpack.Infrastructure;
using Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser;
using Razorvine.Pickle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;


namespace Nodsoft.WowsReplaysUnpack;


public class ReplayUnpacker
{
	public ReplayRaw UnpackReplay(Stream stream)
	{

		byte[] bReplaySignature = new byte[4];
		byte[] bReplayBlockCount = new byte[4];
		byte[] bReplayBlockSize = new byte[4];

		stream.Read(bReplaySignature, 0, 4);
		stream.Read(bReplayBlockCount, 0, 4);
		stream.Read(bReplayBlockSize, 0, 4);

		int jsonDataSize = BitConverter.ToInt32(bReplayBlockSize, 0);

		byte[] bReplayJsonData = new byte[jsonDataSize];
		stream.Read(bReplayJsonData, 0, jsonDataSize);

		ReplayRaw replay = new() { ArenaInfoJson = Encoding.UTF8.GetString(bReplayJsonData) };

		/*
		 * FIXME:
		 * This is the part where we parse the Replay version.
		 * Change to values extracted from the ArenaInfo.
		 */
		IReplayParser replayParser = ReplayParserProvider.FromReplayVersion(Version.Parse("0.10.10"));


		using MemoryStream memStream = new();
		stream.CopyTo(memStream);

		
		

		return replay;
	}

	
}
