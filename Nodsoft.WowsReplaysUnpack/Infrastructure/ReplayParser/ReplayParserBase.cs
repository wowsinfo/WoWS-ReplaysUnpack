using Nodsoft.WowsReplaysUnpack.Data;
using Nodsoft.WowsReplaysUnpack.Data.Raw;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser;

/// <summary>
/// The abstract base class for replay parser implementations.
/// </summary>
public abstract class ReplayParserBase : IReplayParser
{
	public abstract ReplayRaw ParseReplay(MemoryStream memStream, ReplayRaw replay);

	public abstract ReplayPlayer ParseReplayPlayer(ArrayList playerInfo);

	public abstract IEnumerable<ReplayPlayer> ParsePlayersPacket(EntityMethod em);

	public abstract ReplayMessage ParseChatMessagePacket(EntityMethod em);
	
	protected abstract IReplayMessageTypes MessageTypes { get; }
	
	protected abstract IShipConfigMapping ShipConfigMapping { get; }
	
	protected abstract IPlayerMessageMapping PlayerMessageMapping { get; }
}