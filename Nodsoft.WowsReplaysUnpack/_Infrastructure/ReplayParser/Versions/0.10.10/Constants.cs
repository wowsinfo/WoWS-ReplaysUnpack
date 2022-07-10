using Nodsoft.WowsReplaysUnpack.Data;
using System.Collections.Generic;

namespace Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser.Versions;

internal static class Constants_0_10_10
{
	public class PlayerMessageMapping : IPlayerMessageMapping
	{
		public Dictionary<int, string> PropertyMapping { get; } = new()
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
	}

	public class ReplayMessageTypes : IReplayMessageTypes
	{
		public byte OnChatMessage => 122;
		public byte OnArenaStatesReceived => 124;
	}

	/// <summary>
	/// Represents the mapping for the entries of the <see cref="ShipData.ShipConfiguration">ship configuration</see> of a ship.
	/// </summary>
	public class ShipConfigMapping : IShipConfigMapping
	{
		public byte ShipId => 0;
		public byte TotalValueCount => 1;
		public byte ShipModules => 2;
		public byte ShipUpgrades => 3;
		public byte ExteriorSlots => 4;
		public byte AutoSupplyState => 5;
		public byte ColorScheme => 6;
		public byte ConsumableSlots => 7;
		public byte Flags => 8;
	}
}