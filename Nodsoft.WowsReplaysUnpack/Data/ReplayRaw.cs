using System.Collections.Generic;

namespace Nodsoft.WowsReplaysUnpack.Data;


/// <summary>
/// Low-level DTO for Replay file info ingest
/// </summary>
public sealed record ReplayRaw
{
	public string ArenaInfoJson { get; init; }

	public List<ReplayMessage> ChatMessages { get; init; } = new();
	public List<ReplayPlayer> ReplayPlayers { get; init; } = new();


	internal byte[] BReplaySignature { get; init; }
	internal byte[] BReplayBlockCount { get; init; }
	internal byte[] BReplayBlockSize { get; init; }
}
