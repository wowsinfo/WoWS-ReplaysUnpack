using Microsoft.Extensions.DependencyInjection;
using Nodsoft.WowsReplaysUnpack.Controllers;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Services;

namespace Nodsoft.WowsReplaysUnpack;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddWowsReplayUnpacker(this IServiceCollection services)
		=> services.AddWowsReplayUnpacker<DefaultReplayDataParser, DefaultDefinitionStore>();
	public static IServiceCollection AddWowsReplayUnpacker<TReplayDataParser, TDefinitionStore>(this IServiceCollection services)
		where TReplayDataParser : class, IReplayDataParser
		where TDefinitionStore : class, IDefinitionStore
		=> services.AddWowsReplayUnpacker(unpacker =>
																  {
																	  unpacker
																			.WithReplayDataParser<TReplayDataParser>()
																			.WithDefinitionStore<TDefinitionStore>();
																  });
	public static IServiceCollection AddWowsReplayUnpacker(this IServiceCollection services, Action<ReplayUnpackerBuilder> builderAction)
	{
		ReplayUnpackerBuilder builder = new(services);
		builderAction(builder);
		builder.Build();

		services.AddScoped<ReplayUnpackerFactory>();
		return services;
	}



	public class ReplayUnpackerBuilder
	{
		private readonly IServiceCollection _services;
		private bool replayDataParserAdded;
		private bool definitionStoreAdded;

		public ReplayUnpackerBuilder(IServiceCollection services)
		{
			_services = services;
			AddReplayController<DefaultReplayController>();
		}
		public ReplayUnpackerBuilder WithReplayDataParser<T>() where T : class, IReplayDataParser
		{
			_services.AddScoped<IReplayDataParser, T>();
			replayDataParserAdded = true;
			return this;
		}

		public ReplayUnpackerBuilder AddReplayController<T>() where T : class, IReplayController
		{
			_services.AddScoped<ReplayUnpackerService<T>>();
			_services.AddScoped<T>();
			return this;
		}

		public ReplayUnpackerBuilder WithDefinitionStore<T>() where T : class, IDefinitionStore
		{
			_services.AddSingleton<IDefinitionStore, T>();
			definitionStoreAdded = true;
			return this;
		}
		public void Build()
		{
			if (!replayDataParserAdded)
				WithReplayDataParser<DefaultReplayDataParser>();

			if (!definitionStoreAdded)
				WithDefinitionStore<DefaultDefinitionStore>();
		}
	}
}
