using Microsoft.Extensions.DependencyInjection;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Services;
using System;

namespace Nodsoft.WowsReplaysUnpack;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddReplayUnpacker(this IServiceCollection services)
		=> services.AddReplayUnpacker<DefaultReplayDataParser, DefaultReplayController, DefaultDefinitionStore>();
	public static IServiceCollection AddReplayUnpacker<TReplayDataParser, TReplayController, TDefinitionStore>(this IServiceCollection services)
		where TReplayDataParser : class, IReplayDataParser
		where TReplayController : class, IReplayController
		where TDefinitionStore : class, IDefinitionStore
		=> services.AddReplayUnpacker(unpacker =>
																  {
																	  unpacker
																	  .WithReplayDataParser<TReplayDataParser>()
																	  .WithReplayController<TReplayController>()
																	  .WithDefinitionStore<TDefinitionStore>();
																  });
	public static IServiceCollection AddReplayUnpacker(this IServiceCollection services, Action<ReplayUnpackerBuilder> builderAction)
	{
		builderAction(new ReplayUnpackerBuilder(services));
		services.AddScoped<ReplayUnpackerService>();
		return services;
	}



	public class ReplayUnpackerBuilder
	{
		private readonly IServiceCollection _services;

		public ReplayUnpackerBuilder(IServiceCollection services)
		{
			_services = services;
		}
		public ReplayUnpackerBuilder WithReplayDataParser<T>() where T : class, IReplayDataParser
		{
			_services.AddScoped<IReplayDataParser, T>();
			return this;
		}

		public ReplayUnpackerBuilder WithReplayController<T>() where T : class, IReplayController
		{
			_services.AddScoped<IReplayController, T>();
			return this;
		}

		public ReplayUnpackerBuilder WithDefinitionStore<T>() where T : class, IDefinitionStore
		{
			_services.AddSingleton<IDefinitionStore, T>();
			return this;
		}
	}
}
