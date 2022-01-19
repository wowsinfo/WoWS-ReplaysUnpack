namespace Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser.Versions;

internal class ReplayParser_0_11_0 : ReplayParser_0_10_10
{
	protected override IReplayMessageTypes MessageTypes { get; } = new Constants_0_11_0.ReplayMessageTypes();
}
