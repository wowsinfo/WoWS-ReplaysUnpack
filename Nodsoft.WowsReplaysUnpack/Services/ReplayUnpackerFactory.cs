using Microsoft.Extensions.DependencyInjection;
using Nodsoft.WowsReplaysUnpack.Controllers;

namespace Nodsoft.WowsReplaysUnpack.Services;

/// <summary>
/// Represents a factory for creating <see cref="IReplayUnpackerService"/> instances.
/// </summary>
public class ReplayUnpackerFactory
{
	private readonly IServiceProvider _serviceProvider;

	public ReplayUnpackerFactory(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

	/// <summary>
	/// Gets an <see cref="IReplayUnpackerService" /> with the specified <typeparamref name="TController"/>.
	/// </summary>
	/// <typeparam name="TController">The type of the controller.</typeparam>
	/// <returns>An instance of <see cref="IReplayUnpackerService" />.</returns>
	public IReplayUnpackerService GetUnpacker<TController>() where TController : IReplayController
		=> _serviceProvider.GetRequiredService<ReplayUnpackerService<TController>>();

	/// <summary>
	/// Gets the default <see cref="IReplayUnpackerService" />.
	/// </summary>
	/// <returns>An instance of <see cref="IReplayUnpackerService" />.</returns>
	public IReplayUnpackerService GetUnpacker() => GetUnpacker<DefaultReplayController>();
}
