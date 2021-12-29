namespace Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser.Versions;

internal class ReplayParser_0_10_10 : ReplayParserBase
{
	protected override IReplayMessageTypes MessageTypes { get; } = new Constants_0_10_10.ReplayMessageTypes();
	protected override IShipConfigMapping ShipConfigMapping { get; } = new Constants_0_10_10.ShipConfigMapping();
	protected override IPlayerMessageMapping PlayerMessageMapping { get; } = new Constants_0_10_10.PlayerMessageMapping();
}
