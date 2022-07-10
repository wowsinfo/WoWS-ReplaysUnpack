using Nodsoft.WowsReplaysUnpack.Data;
using Nodsoft.WowsReplaysUnpack.Infrastructure;
using Nodsoft.WowsReplaysUnpack.Infrastructure.Exceptions;
using Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser;
using Nodsoft.WowsReplaysUnpack.Models;
using Nodsoft.WowsReplaysUnpack.Models.Replay;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Nodsoft.WowsReplaysUnpack;


public class ReplayUnpackerService
{
	private static readonly byte[] ReplaySignature = Encoding.UTF8.GetBytes("\x12\x32\x34\x11");

	/// <summary>
	/// Unpacks a replay from a stream.
	/// Uses the default <see cref="ReplayParserProvider.Instance">ReplayParserProvider instance</see>.
	/// </summary>
	/// <param name="stream">The stream containing the replay file content.</param>
	/// <returns>The unpacked replay, wrapped in a <see cref="ReplayRaw"/> instance.</returns>
	/// <exception cref="InvalidReplayException">Occurs if the replay file is not valid.</exception>
	public ReplayRaw UnpackReplay(Stream stream)
		=> UnpackReplay(stream, ReplayParserProvider.Instance);

	/// <summary>
	/// Unpacks a replay from a stream.
	/// Uses a custom <see cref="IReplayParserProvider"/> implementation.
	/// </summary>
	/// <param name="stream">The stream containing the replay file content.</param>
	/// <param name="parserProvider">The <see cref="IReplayParserProvider"/> implementation to use to retrieve the necessary <see cref="IReplayParser"/>.</param>
	/// <returns>The unpacked replay, wrapped in a <see cref="ReplayRaw"/> instance.</returns>
	/// <exception cref="InvalidReplayException">Occurs if the replay file is not valid.</exception>
	public ReplayRaw UnpackReplay(Stream stream, IReplayParserProvider parserProvider)
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
		ReplayMetadata metadata = new()
		{
			//ArenaInfo = JsonSerializer.Deserialize<ArenaInfo>(ref reader, options) ?? throw new InvalidReplayException(),
			BReplaySignature = bReplaySignature,
			BReplayBlockCount = bReplayBlockCount,
			BReplayBlockSize = bReplayBlockSize,
		};

		Version replayVersion = Version.Parse(string.Join('.', metadata.ArenaInfo.ClientVersionFromExe.Split(',')[..3]));
		IReplayParser replayParser = parserProvider.FromReplayVersion(replayVersion);

		using MemoryStream memStream = new();
		stream.CopyTo(memStream);
		ReplayRaw replay = replayParser.ParseReplay(memStream, metadata);

		return replay;
	}


	private UnpackedReplay Unpack(Stream stream, ReplayUnpackerOptions? options = null)
	{
		options ??= new();

		using BinaryReader binaryReader = new(stream);
		byte[] replaySignature = binaryReader.ReadBytes(4);
		int replayBlockCount = binaryReader.ReadInt32();

		// Verify replay signature
		if (!replaySignature.SequenceEqual(ReplaySignature))
		{
			throw new InvalidReplayException("Invalid replay signature");
		}

		UnpackedReplay replay = new(ReadArenaInfo(binaryReader));
		//byte[] replayBlockSize = await stream.ReadToArrayAsync(0, 4);

		return replay;
	}

	private ArenaInfo ReadArenaInfo(BinaryReader streamReader)
	{
		int arenaInfoSize = streamReader.ReadInt32();
		byte[] arenaInfoData = streamReader.ReadBytes(arenaInfoSize);
		JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
		options.Converters.Add(new DateTimeJsonConverter());
		Utf8JsonReader jsonReader = new(arenaInfoData);
		return JsonSerializer.Deserialize<ArenaInfo>(ref jsonReader, options) ?? throw new InvalidReplayException();
	}
}
