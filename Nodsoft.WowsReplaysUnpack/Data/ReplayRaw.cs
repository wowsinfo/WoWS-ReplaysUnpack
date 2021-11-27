using System.Collections.Generic;

namespace Nodsoft.WowsReplaysUnpack.Data;


/// <summary>
/// Low-level DTO for Replay file info ingest
/// </summary>
internal record ReplayRaw
{
	public string ArenaInfoJson { get; init; }

	public IReadOnlyList<ReplayMessage> ReplayMessages { get; init; }
	public IEnumerable<ReplayPlayer> ReplayPlayers { get; init; }


	internal byte[] BReplaySignature { get; init; }
	internal byte[] BReplayBlockCount { get; init; }
	internal byte[] BReplayBlockSize { get; init; }
}
