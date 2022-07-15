using Razorvine.Pickle;

namespace Nodsoft.WowsReplaysUnpack.ExtendedData.Models;

public class CamouflageInfo : IObjectConstructor
{
	public object construct(object[] args) => string.Format("{0}, {1}", args);
}