using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Entities;

namespace Nodsoft.WowsReplaysUnpack.Services;
public class DefaultReplayController : AReplayControllerBase<DefaultReplayController>
{
	public DefaultReplayController(IDefinitionStore definitionStore, ILogger<Entity> entityLogger)
		: base(definitionStore, entityLogger)
	{
	}
}
