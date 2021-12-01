using System;
using System.Collections.Generic;
using System.Linq;

namespace Nodsoft.WowsReplaysUnpack.Data;

/// <summary>
/// Represents the data extracted from the shipConfigDump property of a player entry in the replay.
/// </summary>
/// <param name="ShipId">The id of the ship.</param>
/// <param name="ShipConfigurationList">A mapping of the ship configuration.</param>
public sealed record ShipData(uint ShipId, IReadOnlyList<uint> ShipConfigurationList)
{
	private const uint componentIdStart = 3000000000; // Player IDs end at 2.999.999.999 and apparently WG components start at 3 billion.
	
	private readonly Lazy<List<uint>> filteredIds = new(() => ShipConfigurationList.Where(id => id > componentIdStart).ToList());

	/// <summary>
	/// Gets a filtered version of the <see cref="ShipConfigurationList"/> that only contains component IDs used by Wargaming.
	/// Metadata like section length and other configurations are filtered out.
	/// </summary>
	public IReadOnlyList<uint> FilteredIds => filteredIds.Value;
}