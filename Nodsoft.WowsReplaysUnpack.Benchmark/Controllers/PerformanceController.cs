using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Entities;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;
using Nodsoft.WowsReplaysUnpack.Services;

namespace Nodsoft.WowsReplaysUnpack.Benchmark.Controllers
{
	internal class PerformanceController : DefaultReplayController
	{
		public PerformanceController(IDefinitionStore definitionStore, ILogger<Entity> entityLogger) : base(definitionStore, entityLogger)
		{
		}

		public override void HandleNetworkPacket(ANetworkPacket networkPacket)
		{
			if (networkPacket is EntityMethodPacket em)
				OnEntityMethod(em);
		}
	}
}
