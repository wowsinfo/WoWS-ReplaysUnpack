using Nodsoft.WowsReplaysUnpack.ExtendedData.Models;

namespace Nodsoft.WowsReplaysUnpack.ExtendedData.VersionMappings.Mappings;

public class DefaultVersionMapping : IVersionMapping
{
	public virtual Version? Version => null;

	public virtual Dictionary<int, string> ReplayPlayerPropertyMappings => new()
	{
		{ 0, nameof(ReplayPlayer.AccountId) },
		{ 1, nameof(ReplayPlayer.AntiAbuseEnabled) },
		{ 2, nameof(ReplayPlayer.AvatarId) },
		{ 3, nameof(ReplayPlayer.CamouflageInfo) },
		{ 4, nameof(ReplayPlayer.ClanColor) },
		{ 5, nameof(ReplayPlayer.ClanId) },
		{ 6, nameof(ReplayPlayer.ClanTag) },
		{ 7, nameof(ReplayPlayer.CrewParams) },
		{ 8, nameof(ReplayPlayer.DogTag) },
		{ 9, nameof(ReplayPlayer.FragsCount) },
		{ 10, nameof(ReplayPlayer.FriendlyFireEnabled) },
		{ 11, nameof(ReplayPlayer.Id) },
		{ 12, nameof(ReplayPlayer.InvitationsEnabled) },
		{ 13, nameof(ReplayPlayer.IsAbuser) },
		{ 14, nameof(ReplayPlayer.IsAlive) },
		{ 15, nameof(ReplayPlayer.IsBot) },
		{ 16, nameof(ReplayPlayer.IsClientLoaded) },
		{ 17, nameof(ReplayPlayer.IsConnected) },
		{ 18, nameof(ReplayPlayer.IsHidden) },
		{ 19, nameof(ReplayPlayer.IsLeaver) },
		{ 20, nameof(ReplayPlayer.IsPreBattleOwner) },
		{ 21, nameof(ReplayPlayer.IsTShooter) },
		{ 22, nameof(ReplayPlayer.KilledBuildingsCount) },
		{ 23, nameof(ReplayPlayer.MaxHealth) },
		{ 24, nameof(ReplayPlayer.Name) },
		{ 25, nameof(ReplayPlayer.PlayerMode) },
		{ 26, nameof(ReplayPlayer.PreBattleIdOnStart) },
		{ 27, nameof(ReplayPlayer.PreBattleSign) },
		{ 28, nameof(ReplayPlayer.PreBattleId) },
		{ 29, nameof(ReplayPlayer.Realm) },
		{ 30, nameof(ReplayPlayer.ShipComponents) },
		{ 31, nameof(ReplayPlayer.ShipConfigDump) },
		{ 32, nameof(ReplayPlayer.ShipId) },
		{ 33, nameof(ReplayPlayer.ShipParamsId) },
		{ 34, nameof(ReplayPlayer.SkinId) },
		{ 35, nameof(ReplayPlayer.TeamId) },
		{ 36, nameof(ReplayPlayer.TtkStatus) }
	};

	public ShipConfigMapping ShipConfigMapping { get; } = new ShipConfigMapping
	{
		ShipId = 0,
		TotalValueCount = 1,
		ShipModules = 2,
		ShipUpgrades = 3,
		ExteriorSlots = 4,
		AutoSupplyState = 5,
		ColorScheme = 6,
		ConsumableSlots = 7,
		Flags = 8,
	};
}
