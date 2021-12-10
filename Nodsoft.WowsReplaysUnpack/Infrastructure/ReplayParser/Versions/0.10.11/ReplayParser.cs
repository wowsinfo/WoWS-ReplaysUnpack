using Nodsoft.WowsReplaysUnpack.Data;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System;
using System.Linq;
using Mapster;
using Nodsoft.WowsReplaysUnpack.Data.Raw;
using Razorvine.Pickle;
using System.IO.Compression;
using System.IO;
using System.Text;

namespace Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser.Versions;

internal class ReplayParser_0_10_11 : ReplayParser_0_10_10
{
	protected override IReplayMessageTypes MessageTypes { get; } = new Constants_0_10_11.ReplayMessageTypes();
}
