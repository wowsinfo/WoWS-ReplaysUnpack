using Microsoft.Extensions.DependencyInjection;
using Nodsoft.WowsReplaysUnpack.Services;

namespace Nodsoft.WowsReplaysUnpack;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddReplayUnpacker(this IServiceCollection services)
		=> AddReplayUnpacker<DefaultReplayController>(services);

	public static IServiceCollection AddReplayUnpacker<ReplayController>(this IServiceCollection services) where ReplayController : class, IReplayController
	{
		services.AddSingleton<ReplayUnpackerService>();
		services.AddSingleton<CveSecurityService>();
		services.AddScoped<IReplayDataParser, ReplayDataParser>();
		services.AddScoped<IReplayController, ReplayController>();
		return services;
	}
}
