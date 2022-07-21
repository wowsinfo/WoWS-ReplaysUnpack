using Microsoft.Extensions.DependencyInjection;
using Nodsoft.WowsReplaysUnpack.Controllers;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Services;

namespace Nodsoft.WowsReplaysUnpack;

/// <summary>
/// Various extension methods for Dependency Injection.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Registers the WOWS replay data unpacker,
	/// using the <see cref="DefaultReplayDataParser"/> and <see cref="DefaultDefinitionStore"/>
	/// for parsing the replay data and definitions.
	/// </summary>
	/// <param name="services">The service collection.</param>
	/// <returns>The service collection.</returns>
	public static IServiceCollection AddWowsReplayUnpacker(this IServiceCollection services) 
		=> services.AddWowsReplayUnpacker<DefaultReplayDataParser, DefaultDefinitionStore>();
	
	/// <summary>
	/// Registers the WOWS replay data unpacker,
	/// using the specified <see cref="IReplayDataParser"/> and <see cref="IDefinitionStore"/>
	/// for parsing the replay data and definitions.
	/// </summary>
	/// <param name="services">The service collection to add the services to.</param>
	/// <typeparam name="TReplayDataParser">The type of the replay data parser.</typeparam>
	/// <typeparam name="TDefinitionStore">The type of the definition store.</typeparam>
	/// <returns>The service collection.</returns>
	public static IServiceCollection AddWowsReplayUnpacker<TReplayDataParser, TDefinitionStore>(this IServiceCollection services)
		where TReplayDataParser : class, IReplayDataParser
		where TDefinitionStore : class, IDefinitionStore
		=> services.AddWowsReplayUnpacker(unpacker =>
			{
				unpacker
					.WithReplayDataParser<TReplayDataParser>()
					.WithDefinitionStore<TDefinitionStore>();
			}
		);
	
	/// <summary>
	/// Builds the WOWS replay data unpacker using the <see cref="ReplayUnpackerFactory"/>,
	/// and a builder action to configure the unpacker.
	/// </summary>
	/// <param name="services">The service collection to add the services to.</param>
	/// <param name="builderAction">The builder action to configure the unpacker.</param>
	/// <returns>The service collection.</returns>
	public static IServiceCollection AddWowsReplayUnpacker(this IServiceCollection services, Action<ReplayUnpackerBuilder> builderAction)
	{
		ReplayUnpackerBuilder builder = new(services);
		builderAction(builder);
		builder.Build();

		services.AddScoped<ReplayUnpackerFactory>();
		return services;
	}

	/// <summary>
	/// Provides a fluent API to build a WOWS replay data unpacker.
	/// </summary>
	public class ReplayUnpackerBuilder
	{
		private bool replayDataParserAdded;
		private bool definitionStoreAdded;

		public IServiceCollection Services { get; }

		/// <summary>
		/// Intializes a new instance of the <see cref="ReplayUnpackerBuilder" /> class,
		/// by registering a <see cref="ReplayUnpackerService" /> as baseline.
		/// </summary>
		/// <param name="services"></param>
		public ReplayUnpackerBuilder(IServiceCollection services)
		{
			Services = services;
			AddReplayController<DefaultReplayController>();
		}
		
		/// <summary>
		/// Registers a <see cref="IReplayDataParser" /> for use in the WOWS replay data unpacker.
		/// </summary>
		/// <typeparam name="TParser">The type of the replay data parser.</typeparam>
		/// <returns>The builder.</returns>
		public ReplayUnpackerBuilder WithReplayDataParser<TParser>() where TParser : class, IReplayDataParser
		{
			Services.AddScoped<IReplayDataParser, TParser>();
			replayDataParserAdded = true;
			return this;
		}

		/// <summary>
		/// Registers a <see cref="IReplayController" /> for use in the WOWS replay data unpacker.
		/// </summary>
		/// <typeparam name="TController">The type of the replay controller.</typeparam>
		/// <returns>The builder.</returns>
		public ReplayUnpackerBuilder AddReplayController<TController>() where TController : class, IReplayController
		{
			Services.AddScoped<ReplayUnpackerService<TController>>();
			Services.AddScoped<TController>();
			return this;
		}

		/// <summary>
		/// Registers a <see cref="IDefinitionStore" /> for use in the WOWS replay data unpacker.
		/// </summary>
		/// <typeparam name="TStore">The type of the definition store.</typeparam>
		/// <returns>The builder.</returns>
		public ReplayUnpackerBuilder WithDefinitionStore<TStore>() where TStore : class, IDefinitionStore
		{
			Services.AddSingleton<IDefinitionStore, TStore>();
			definitionStoreAdded = true;
			return this;
		}
		
		/// <summary>
		/// Builds the WOWS replay data unpacker, registering any missing services.
		/// </summary>
		public void Build()
		{
			if (!replayDataParserAdded)
			{
				WithReplayDataParser<DefaultReplayDataParser>();
			}

			if (!definitionStoreAdded)
			{
				WithDefinitionStore<DefaultDefinitionStore>();
			}
		}
	}
}
