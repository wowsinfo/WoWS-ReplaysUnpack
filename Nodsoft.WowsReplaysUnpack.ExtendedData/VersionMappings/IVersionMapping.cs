using Nodsoft.WowsReplaysUnpack.ExtendedData.Models;

namespace Nodsoft.WowsReplaysUnpack.ExtendedData.VersionMappings;

public interface IVersionMapping
{
	Version? Version { get; }
	Dictionary<int, string> ReplayPlayerPropertyMappings { get; }
	ShipConfigMapping ShipConfigMapping { get; }
}
