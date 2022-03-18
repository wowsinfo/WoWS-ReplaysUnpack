namespace Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser.Versions;

internal static class Constants_0_11_2
{
	public class ReplayMessageTypes : IReplayMessageTypes
	{
		public byte OnChatMessage { get; } = 124;
		public byte OnArenaStatesReceived { get; } = 126;
	}
}