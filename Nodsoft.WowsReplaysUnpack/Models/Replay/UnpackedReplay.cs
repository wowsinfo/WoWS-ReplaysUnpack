using System.Collections.Generic;

namespace Nodsoft.WowsReplaysUnpack.Models.Replay;

public sealed record UnpackedReplay
{
	public ArenaInfo ArenaInfo { get; set; }

	public UnpackedReplay(ArenaInfo arenaInfo)
	{
		ArenaInfo = arenaInfo;
	}
	//public ReplayMetadata ReplayMetadata { get; }
	//public List<ReplayMessage> ChatMessages { get; init; } = new();
	//public List<ReplayPlayer> ReplayPlayers { get; init; } = new();
}
