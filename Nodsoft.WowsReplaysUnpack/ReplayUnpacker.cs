using Nodsoft.WowsReplaysUnpack.Data;
using Nodsoft.WowsReplaysUnpack.Infrastructure;
using Nodsoft.WowsReplaysUnpack.Infrastructure.Exceptions;
using Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Nodsoft.WowsReplaysUnpack;


public class ReplayUnpacker
{
	private static readonly byte[] ReplaySignature = Encoding.UTF8.GetBytes("\x12\x32\x34\x11");
	
	public ReplayRaw UnpackReplay(Stream stream)
	{

		byte[] bReplaySignature = new byte[4];
		byte[] bReplayBlockCount = new byte[4];
		byte[] bReplayBlockSize = new byte[4];

		stream.Read(bReplaySignature, 0, 4);
		stream.Read(bReplayBlockCount, 0, 4);
		stream.Read(bReplayBlockSize, 0, 4);

		// Verify replay signature
		if (!bReplaySignature.SequenceEqual(ReplaySignature))
		{
			throw new InvalidReplayException("Invalid replay signature.");
		}

		int jsonDataSize = BitConverter.ToInt32(bReplayBlockSize, 0);
		byte[] bReplayJsonData = new byte[jsonDataSize];
		stream.Read(bReplayJsonData, 0, jsonDataSize);

		JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
		options.Converters.Add(new DateTimeJsonConverter());
		Utf8JsonReader reader = new(bReplayJsonData);
		ReplayRaw replay = new()
		{
			ArenaInfo = JsonSerializer.Deserialize<ArenaInfo>(ref reader, options) ?? throw new InvalidReplayException(),
			BReplaySignature = bReplaySignature,
			BReplayBlockCount = bReplayBlockCount,
			BReplayBlockSize = bReplayBlockSize,
		};
		
		string replayVersionString = string.Join('.', replay.ArenaInfo.ClientVersionFromExe.Split(',')[..3]);
		Version replayVersion = Version.Parse(replayVersionString);
		IReplayParser replayParser = ReplayParserProvider.FromReplayVersion(replayVersion);


		using MemoryStream memStream = new();
		stream.CopyTo(memStream);
		replay = replayParser.ParseReplay(memStream, replay);

		return replay;
	}

	
}
