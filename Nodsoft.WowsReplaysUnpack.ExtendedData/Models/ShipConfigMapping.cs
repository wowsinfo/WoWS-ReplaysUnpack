namespace Nodsoft.WowsReplaysUnpack.ExtendedData.Models;

public class ShipConfigMapping
{
	public byte ShipId { get; set; }
	public byte TotalValueCount { get; set; }
	public byte ShipModules { get; set; }
	public byte ShipUpgrades { get; set; }
	public byte ExteriorSlots { get; set; }
	public byte AutoSupplyState { get; set; }
	public byte ColorScheme { get; set; }
	public byte ConsumableSlots { get; set; }
	public byte Flags { get; set; }
}
