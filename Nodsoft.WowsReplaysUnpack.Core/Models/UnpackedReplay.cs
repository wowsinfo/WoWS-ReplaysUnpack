using Nodsoft.WowsReplaysUnpack.Core.Entities;
using System.Text.Json;

namespace Nodsoft.WowsReplaysUnpack.Core.Models;

public class UnpackedReplay
{
	public Version ClientVersion { get; }
	public ArenaInfo ArenaInfo { get; }
	public List<JsonElement> ExtraJsonData { get; } = new();
	public Dictionary<uint, Entity> Entities { get; } = new();
	public uint? PlayerEntityId { get; set; }
	public string? MapName { get; set; }

	public UnpackedReplay(ArenaInfo arenaInfo)
	{
		ArenaInfo = arenaInfo;
		ClientVersion = Version.Parse(string.Join('.', ArenaInfo.ClientVersionFromExe.Split(',')[..3]));
	}
}
