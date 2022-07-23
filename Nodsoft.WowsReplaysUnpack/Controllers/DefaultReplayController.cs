using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Entities;

namespace Nodsoft.WowsReplaysUnpack.Controllers;

/// <summary>
/// Default implementation of the <see cref="ReplayControllerBase{TController}"/>.
/// </summary>
public sealed class DefaultReplayController : ReplayControllerBase<DefaultReplayController>
{
	// ReSharper disable once ContextualLoggerProblem
	public DefaultReplayController(IDefinitionStore definitionStore, ILogger<Entity> entityLogger) : base(definitionStore, entityLogger) { }
}