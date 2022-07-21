using Microsoft.Extensions.DependencyInjection;
using Nodsoft.WowsReplaysUnpack.Controllers;

namespace Nodsoft.WowsReplaysUnpack.Services;

public class ReplayUnpackerFactory
{
	private readonly IServiceProvider _serviceProvider;

	public ReplayUnpackerFactory(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public IReplayUnpackerService GetUnpacker<TController>() where TController : IReplayController
		=> _serviceProvider.GetRequiredService<ReplayUnpackerService<TController>>();

	public IReplayUnpackerService GetUnpacker()
	 => GetUnpacker<DefaultReplayController>();
}
