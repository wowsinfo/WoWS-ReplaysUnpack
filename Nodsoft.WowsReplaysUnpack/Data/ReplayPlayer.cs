using System.Collections.Generic;

namespace Nodsoft.WowsReplaysUnpack.Data;

public sealed record ReplayPlayer
{
	public IReadOnlyDictionary<string, object> Properties { get; init; }

	public object AccountDBID { get; set; }
	public object AvatarId { get; set; }
	public object CamouflageInfo { get; set; }
	public object ClanColor { get; set; }
	public object ClanID { get; set; }
	public object ClanTag { get; set; }
	public object CrewParams { get; set; }
	public object DogTag { get; set; }
	public object FragsCount { get; set; }
	public object FriendlyFireEnabled { get; set; }
	public object Id { get; set; }
	public object InvitationsEnabled { get; set; }
	public object IsAbuser { get; set; }
	public object IsAlive { get; set; }
	public object IsBot { get; set; }
	public object IsClientLoaded { get; set; }
	public object IsConnected { get; set; }
	public object IsHidden { get; set; }
	public object IsLeaver { get; set; }
	public object IsPreBattleOwner { get; set; }
	public object KilledBuildingsCount { get; set; }
	public object MaxHealth { get; set; }
	public object Name { get; set; }
	public object PlayerMode { get; set; }
	public object PreBattleIdOnStart { get; set; }
	public object PreBattleSign { get; set; }
	public object PrebattleId { get; set; }
	public object Realm { get; set; }
	public object ShipComponents { get; set; }
	public object ShipId { get; set; }
	public object ShipParamsId { get; set; }
	public object SkinId { get; set; }
	public object TeamId { get; set; }
	public object TtkStatus { get; set; }

}
