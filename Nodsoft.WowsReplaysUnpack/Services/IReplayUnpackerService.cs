using Nodsoft.WowsReplaysUnpack.Core.Models;

namespace Nodsoft.WowsReplaysUnpack.Services;

public interface IReplayUnpackerService
{
	UnpackedReplay Unpack(byte[] data, ReplayUnpackerOptions? options = null);
	UnpackedReplay Unpack(Stream stream, ReplayUnpackerOptions? options = null);
}