using Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser;
using Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser.Versions;
using System.Collections.Generic;
using System.Linq;

namespace Nodsoft.WowsReplaysUnpack.Data;

/*
 * HACK: Set Ship Constants to latest version constants.
 * This shoud be fixed ASAP.
 */


/// <summary>
/// Represents the data extracted from the shipConfigDump property of a player entry in the replay.
/// <br/>
/// The individual lists may contain 0 values, indicating that a slot or module is either unused or not available on the ship.
/// </summary>
/// <param name="ShipConfiguration">A mapping of the ship configuration.</param>
public sealed record ShipData(IReadOnlyList<IReadOnlyList<uint>> ShipConfiguration)
{
	static IShipConfigMapping ShipConfigMapping { get; } = new Constants_0_10_10.ShipConfigMapping();

	/// <summary>
	/// Gets the numeric id of the selected ship.
	/// </summary>
	public uint ShipId { get; } = ShipConfiguration[ShipConfigMapping.ShipId][0];

	/// <summary>
	/// Gets the total number of values that are present in the raw data list.
	/// Not recommended to use.
	/// </summary>
	public uint TotalValueCount { get; } = ShipConfiguration[ShipConfigMapping.TotalValueCount][0];

	/// <summary>
	/// Gets the list of the ids of the selected ship modules.
	/// </summary>
	public IReadOnlyList<uint> ShipModules { get; } = ShipConfiguration[ShipConfigMapping.ShipModules];

	/// <summary>
	/// Gets the list of the ids of the selected ship upgrades.
	/// </summary>
	public IReadOnlyList<uint> ShipUpgrades { get; } = ShipConfiguration[ShipConfigMapping.ShipUpgrades];

	/// <summary>
	/// Gets the list of the selected exterior components.
	/// Usually signals, but it can contain other exterior stuff as well.
	/// </summary>
	public IReadOnlyList<uint> ExteriorSlots { get; } = ShipConfiguration[ShipConfigMapping.ExteriorSlots];

	/// <summary>
	/// Gets the auto supply state. It's unclear how this is calculated.
	/// </summary>
	public uint AutoSupplyState { get; } = ShipConfiguration[ShipConfigMapping.AutoSupplyState][0];

	/// <summary>
	/// Gets the list of color scheme data.
	/// </summary>
	public IReadOnlyList<uint> ColorScheme { get; } = ShipConfiguration[ShipConfigMapping.ColorScheme];

	/// <summary>
	/// Gets the list of the selected consumables.
	/// </summary>
	public IReadOnlyList<uint> ConsumableSlots { get; } = ShipConfiguration[ShipConfigMapping.ConsumableSlots];

	/// <summary>
	/// Gets the currently mounted flags of the ship.
	/// </summary>
	public IReadOnlyList<uint> Flags { get; } = ShipConfiguration[ShipConfigMapping.Flags];
}