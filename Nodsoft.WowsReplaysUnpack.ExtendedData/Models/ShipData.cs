namespace Nodsoft.WowsReplaysUnpack.ExtendedData.Models;

/// <summary>
/// Represents the data extracted from the shipConfigDump property of a player entry in the replay.
/// <br/>
/// The individual lists may contain 0 values, indicating that a slot or module is either unused or not available on the ship.
/// </summary>
/// <param name="ShipConfiguration">A list of the ship configuration.</param>
/// <param name="ShipConfigMapping">The mapping used to retrieve values from the provided ship configuration.</param>
public sealed record ShipData(IReadOnlyList<uint[]> ShipConfiguration, ShipConfigMapping ShipConfigMapping)
{
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
	public uint[] ShipModules { get; } = ShipConfiguration[ShipConfigMapping.ShipModules];

	/// <summary>
	/// Gets the list of the ids of the selected ship upgrades.
	/// </summary>
	public uint[] ShipUpgrades { get; } = ShipConfiguration[ShipConfigMapping.ShipUpgrades];

	/// <summary>
	/// Gets the list of the selected exterior components.
	/// Usually signals, but it can contain other exterior stuff as well.
	/// </summary>
	public uint[] ExteriorSlots { get; } = ShipConfiguration[ShipConfigMapping.ExteriorSlots];

	/// <summary>
	/// Gets the auto supply state. It's unclear how this is calculated.
	/// </summary>
	public uint AutoSupplyState { get; } = ShipConfiguration[ShipConfigMapping.AutoSupplyState].ElementAtOrDefault(0);

	/// <summary>
	/// Gets the list of color scheme data.
	/// </summary>
	public uint[] ColorScheme { get; } = ShipConfiguration[ShipConfigMapping.ColorScheme];

	/// <summary>
	/// Gets the list of the selected consumables.
	/// </summary>
	public uint[] ConsumableSlots { get; } = ShipConfiguration[ShipConfigMapping.ConsumableSlots];

	/// <summary>
	/// Gets the currently mounted flags of the ship.
	/// </summary>
	public uint[] Flags { get; } = ShipConfiguration[ShipConfigMapping.Flags];
}
