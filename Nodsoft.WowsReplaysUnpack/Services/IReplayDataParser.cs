using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;
using Nodsoft.WowsReplaysUnpack.Core.Models;

namespace Nodsoft.WowsReplaysUnpack.Services;

public interface IReplayDataParser : IDisposable
{
	IEnumerable<ANetworkPacket> ParseNetworkPackets(MemoryStream replayDataStream, ReplayUnpackerOptions options);
}