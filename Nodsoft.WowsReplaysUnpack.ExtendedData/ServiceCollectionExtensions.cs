using Microsoft.Extensions.DependencyInjection;
using Nodsoft.WowsReplaysUnpack.ExtendedData.VersionMappings;
using static Nodsoft.WowsReplaysUnpack.ServiceCollectionExtensions;

namespace Nodsoft.WowsReplaysUnpack.ExtendedData;

public static class ServiceCollectionExtensions
{
	public static ReplayUnpackerBuilder AddExtendedData(this ReplayUnpackerBuilder builder)
	{
		builder.AddReplayController<ExtendedDataController>();
		builder.Services.AddSingleton<VersionMappingFactory>();
		foreach (var versionMappingType in VersionMappingFactory.VersionMappingTypes)
			builder.Services.AddSingleton(typeof(IVersionMapping), versionMappingType);
		return builder;
	}
}
