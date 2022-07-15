using Microsoft.Extensions.DependencyInjection;

namespace Nodsoft.WowsReplaysUnpack.ExtendedData.VersionMappings;

public sealed class VersionMappingFactory
{
	public static Type[] VersionMappingTypes { get; }

	private readonly IServiceProvider _serviceProvider;

	static VersionMappingFactory()
	{
		VersionMappingTypes = typeof(VersionMappingFactory).Assembly.GetTypes()
			 .Where(t => t.Namespace!.Contains("VersionMappings") && t != typeof(IVersionMapping) && t.IsAssignableTo(typeof(IVersionMapping)))
			 .ToArray();
	}

	public VersionMappingFactory(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}



	public IVersionMapping GetMappings(Version version)
	{
		return _serviceProvider.GetServices<IVersionMapping>()
			.OrderByDescending(mapping => mapping.Version)
			.First(mapping => version >= mapping.Version || mapping.Version is null);
	}
}
