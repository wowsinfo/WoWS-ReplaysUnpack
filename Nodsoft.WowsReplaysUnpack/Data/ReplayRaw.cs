using System.Collections.Generic;

namespace Nodsoft.WowsReplaysUnpack.Data;


/// <summary>
/// Low-level DTO for Replay file info ingest
/// </summary>
public sealed record ReplayRaw
{
	internal ReplayRaw(ReplayMetadata replayMetadata)
	{
		ReplayMetadata = replayMetadata;
	}
	
	public ReplayMetadata ReplayMetadata { get; }
	public List<ReplayMessage> ChatMessages { get; init; } = new();
	public List<ReplayPlayer> ReplayPlayers { get; init; } = new();
}
