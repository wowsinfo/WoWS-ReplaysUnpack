using Nodsoft.WowsReplaysUnpack.Services;

namespace Nodsoft.WowsReplaysUnpack.ExtendedData;

public static class Extensions
{
	public static IReplayUnpackerService GetExtendedDataUnpacker(this ReplayUnpackerFactory factory) => factory.GetUnpacker<ExtendedDataController>();
}
