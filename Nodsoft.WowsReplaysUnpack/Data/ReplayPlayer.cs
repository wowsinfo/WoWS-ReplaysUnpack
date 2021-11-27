using System.Collections.Generic;

namespace Nodsoft.WowsReplaysUnpack.Data;

internal record ReplayPlayer
{
	IEnumerable<object> Properties { get; init; }
}
