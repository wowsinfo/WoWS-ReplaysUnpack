namespace Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser.Versions;

internal static class Constants_0_11_0
{
	public class ReplayMessageTypes : IReplayMessageTypes
	{
		public byte OnChatMessage { get; } = 122;
		public byte OnArenaStatesReceived { get; } = 124;
	}
}