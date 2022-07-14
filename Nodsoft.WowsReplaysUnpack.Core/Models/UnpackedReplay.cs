using Nodsoft.WowsReplaysUnpack.Core.Entities;
using System.Text.Json;

namespace Nodsoft.WowsReplaysUnpack.Core.Models;

public class UnpackedReplay
{
	public Version ClientVersion { get; }
	public ArenaInfo ArenaInfo { get; }
	public List<JsonElement> ExtraJsonData { get; } = new();
	public List<Entity> Entities { get; } = new();

	public UnpackedReplay(ArenaInfo arenaInfo)
	{
		ArenaInfo = arenaInfo;
		ClientVersion = Version.Parse(string.Join('.', ArenaInfo.ClientVersionFromExe.Split(',')[..3]));
	}
	//public ReplayMetadata ReplayMetadata { get; }
	//public List<ReplayMessage> ChatMessages { get; init; } = new();
	//public List<ReplayPlayer> ReplayPlayers { get; init; } = new();
}
