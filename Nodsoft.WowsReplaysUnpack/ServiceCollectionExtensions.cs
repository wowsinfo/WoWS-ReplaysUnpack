using Microsoft.Extensions.DependencyInjection;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Services;

namespace Nodsoft.WowsReplaysUnpack;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddReplayUnpacker(this IServiceCollection services)
		=> AddReplayUnpacker<DefaultReplayController>(services);

	public static IServiceCollection AddReplayUnpacker<ReplayController>(this IServiceCollection services)
		where ReplayController : class, IReplayController
	{
		services.AddScoped<ReplayUnpackerService>();
		services.AddScoped<IReplayDataParser, ReplayDataParser>();
		services.AddScoped<IReplayController, ReplayController>();

		services.AddSingleton<DefinitionStore>();
		return services;
	}
}
