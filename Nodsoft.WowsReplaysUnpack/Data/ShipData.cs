using System;
using System.Collections.Generic;
using System.Linq;

namespace Nodsoft.WowsReplaysUnpack.Data;

/// <summary>
/// Represents the data extracted from the shipConfigDump property of a player entry in the replay.
/// </summary>
/// <param name="ShipId">The id of the ship.</param>
/// <param name="ShipConfigurationList">A mapping of the ship configuration.</param>
public record ShipData(long ShipId, IReadOnlyList<long> ShipConfigurationList)
{
	private readonly Lazy<List<long>> filteredIds = new(() => ShipConfigurationList.Where(id => id > 3_000_000_000).ToList());

	/// <summary>
	/// Gets a filtered version of the <see cref="ShipConfigurationList"/> that only contains component IDs used by Wargaming.
	/// Metadata like section length and other configurations are filtered out.
	/// </summary>
	public IReadOnlyList<long> FilteredIds => filteredIds.Value;
}