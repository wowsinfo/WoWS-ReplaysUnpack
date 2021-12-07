using Nodsoft.WowsReplaysUnpack.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser;


public interface IReplayParser
{
	public ReplayRaw ParseReplay(MemoryStream memStream, ReplayRaw replay);
	public ReplayPlayer ParseReplayPlayer(ArrayList playerInfo);


	public IEnumerable<ReplayPlayer> ParsePlayersPacket(EntityMethod em);
	public ReplayMessage ParseChatMessagePacket(EntityMethod em);
}
