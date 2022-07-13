using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Nodsoft.WowsReplaysUnpack.Services;

public interface IReplayDataParser : IDisposable
{
	IEnumerable<INetworkPacket> ParseNetworkPackets(MemoryStream replayDataStream, ReplayUnpackerOptions options);
}