using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;
using Nodsoft.WowsReplaysUnpack.Core.Models;

namespace Nodsoft.WowsReplaysUnpack.Services;

public interface IReplayDataParser : IDisposable
{
	IEnumerable<NetworkPacketBase> ParseNetworkPackets(MemoryStream replayDataStream, ReplayUnpackerOptions options);
}