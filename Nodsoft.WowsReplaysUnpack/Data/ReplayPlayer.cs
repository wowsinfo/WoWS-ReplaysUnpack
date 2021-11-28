using Razorvine.Pickle.Objects;
using System.Collections;
using System.Collections.Generic;

namespace Nodsoft.WowsReplaysUnpack.Data;

public sealed record ReplayPlayer
{
	public IReadOnlyDictionary<string, object> Properties { get; init; }

	public uint AccountDBID { get; set; }
	public uint AvatarId { get; set; }

	public object CamouflageInfo { get; set; }

	public uint ClanColor { get; set; }
	public uint ClanID { get; set; }
	public string ClanTag { get; set; }

	public ArrayList CrewParams { get; set; }
	public ArrayList DogTag { get; set; }
	public short FragsCount { get; set; }
	public bool FriendlyFireEnabled { get; set; }
	public uint Id { get; set; }
	public bool InvitationsEnabled { get; set; }
	public bool IsAbuser { get; set; }
	public bool IsAlive { get; set; }
	public bool IsBot { get; set; }
	public bool IsClientLoaded { get; set; }
	public bool IsConnected { get; set; }
	public bool IsHidden { get; set; }
	public bool IsLeaver { get; set; }
	public bool IsPreBattleOwner { get; set; }
	public short KilledBuildingsCount { get; set; }
	public uint MaxHealth { get; set; }
	public string Name { get; set; }
	public ClassDict PlayerMode { get; set; }
	public uint PreBattleIdOnStart { get; set; }
	public object PreBattleSign { get; set; }
	public uint PrebattleId { get; set; }
	public string Realm { get; set; }
	public Hashtable ShipComponents { get; set; }
	public uint ShipId { get; set; }
	public uint ShipParamsId { get; set; }
	public uint SkinId { get; set; }
	public uint TeamId { get; set; }
	public bool TtkStatus { get; set; }

}
