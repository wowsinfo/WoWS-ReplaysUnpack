using Nodsoft.WowsReplaysUnpack.Data;
using Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser;
using System.Collections.Generic;

namespace Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser.Versions;

internal static class Constants_0_10_11
{
	public class ReplayMessageTypes : IReplayMessageTypes
	{
		public byte OnChatMessage => 121;
		public byte OnArenaStatesReceived => 123;
	}
}