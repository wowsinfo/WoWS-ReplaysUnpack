using System.Collections.Generic;

namespace Nodsoft.WowsReplaysUnpack.Data;

public sealed record ReplayPlayer
{
	public IEnumerable<object> Properties { get; init; }
}
