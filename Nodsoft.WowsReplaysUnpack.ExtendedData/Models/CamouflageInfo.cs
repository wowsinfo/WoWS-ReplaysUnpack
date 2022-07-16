using Razorvine.Pickle;

namespace Nodsoft.WowsReplaysUnpack.ExtendedData.Models;

public record CamouflageInfo : IObjectConstructor
{
	public object construct(object[] args) => string.Format("{0}, {1}", args);
}