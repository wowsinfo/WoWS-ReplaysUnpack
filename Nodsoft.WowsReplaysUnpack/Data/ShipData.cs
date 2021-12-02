using System.Collections.Generic;
using System.Linq;

namespace Nodsoft.WowsReplaysUnpack.Data;

/// <summary>
/// Represents the data extracted from the shipConfigDump property of a player entry in the replay.
/// <br/>
/// The individual lists may contain 0 values, indicating that a slot or module is either unused or not available on the ship.
/// </summary>
/// <param name="ShipConfiguration">A mapping of the ship configuration.</param>
public sealed record ShipData(IReadOnlyList<IReadOnlyList<uint>> ShipConfiguration)
{
	/// <summary>
	/// Gets the numeric id of the selected ship.
	/// </summary>
	public uint ShipId => ShipConfiguration[Constants.ShipConfigMapping.ShipId].First();

	/// <summary>
	/// Gets the total number of values that are present in the raw data list.
	/// Not recommended to use.
	/// </summary>
	public uint TotalValueCount => ShipConfiguration[Constants.ShipConfigMapping.TotalValueCount].First();

	/// <summary>
	/// Gets the list of the ids of the selected ship modules.
	/// </summary>
	public IReadOnlyList<uint> ShipModules => ShipConfiguration[Constants.ShipConfigMapping.ShipModules];

	/// <summary>
	/// Gets the list of the ids of the selected ship upgrades.
	/// </summary>
	public IReadOnlyList<uint> ShipUpgrades => ShipConfiguration[Constants.ShipConfigMapping.ShipUpgrades];

	/// <summary>
	/// Gets the list of the selected exterior components.
	/// Usually signals, but it can contain other exterior stuff as well.
	/// </summary>
	public IReadOnlyList<uint> ExteriorSlots => ShipConfiguration[Constants.ShipConfigMapping.ExteriorSlots];

	/// <summary>
	/// Gets the auto supply state. It's unclear how this is calculated.
	/// </summary>
	public uint AutoSupplyState => ShipConfiguration[Constants.ShipConfigMapping.AutoSupplyState].First();

	/// <summary>
	/// Gets the list of color scheme data.
	/// </summary>
	public IReadOnlyList<uint> ColorScheme => ShipConfiguration[Constants.ShipConfigMapping.ColorScheme];

	/// <summary>
	/// Gets the list of the selected consumables.
	/// </summary>
	public IReadOnlyList<uint> ConsumableSlots => ShipConfiguration[Constants.ShipConfigMapping.ConsumableSlots];

	/// <summary>
	/// Gets the currently mounted flags of the ship.
	/// </summary>
	public IReadOnlyList<uint> Flags => ShipConfiguration[Constants.ShipConfigMapping.Flags];
}