namespace Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser.Versions;

internal class ReplayParser_0_10_11 : ReplayParser_0_10_10
{
	protected override IReplayMessageTypes MessageTypes { get; } = new Constants_0_10_11.ReplayMessageTypes();
}
