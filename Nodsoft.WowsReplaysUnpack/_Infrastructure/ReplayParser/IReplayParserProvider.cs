using System;

namespace Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser;

public interface IReplayParserProvider
{
	public IReplayParser FromReplayVersion(Version version);
}