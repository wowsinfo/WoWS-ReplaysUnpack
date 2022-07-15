using Nodsoft.WowsReplaysUnpack.Core.Models;

namespace Nodsoft.WowsReplaysUnpack.ExtendedData.Models;

public class ExtendedDataReplay : UnpackedReplay
{
	public ExtendedDataReplay(ArenaInfo arenaInfo) : base(arenaInfo)
	{
	}

	public List<ReplayPlayer> ReplayPlayers { get; set; } = new();
	public List<ChatMessage> ChatMessages { get; set; } = new();
}
