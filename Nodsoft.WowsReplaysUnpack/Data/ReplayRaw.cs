using System;
using System.Collections.Generic;

namespace Nodsoft.WowsReplaysUnpack.Data;


/// <summary>
/// Low-level DTO for Replay file info ingest
/// </summary>
public sealed record ReplayRaw
{
	internal ReplayRaw()
	{
	}
	
	public ArenaInfo ArenaInfo { get; internal init; } = default!;

	public List<ReplayMessage> ChatMessages { get; init; } = new();
	public List<ReplayPlayer> ReplayPlayers { get; init; } = new();


	internal byte[] BReplaySignature { get; init; } = Array.Empty<byte>();
	internal byte[] BReplayBlockCount { get; init; } = Array.Empty<byte>();
	internal byte[] BReplayBlockSize { get; init; } = Array.Empty<byte>();
}
