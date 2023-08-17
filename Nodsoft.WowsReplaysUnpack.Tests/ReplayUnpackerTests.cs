using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Exceptions;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Services;
using Xunit;


/*
 * FIXME: Test parallelization is disabled due to a file loading issue.
 */
[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Nodsoft.WowsReplaysUnpack.Tests;

public sealed class ReplayUnpackerTests
{
	private readonly ReplayUnpackerFactory _factory;
	private readonly string _sampleFolder = Path.Join(Directory.GetCurrentDirectory(), "Replay-Samples");

	public ReplayUnpackerTests()
	{
		_factory = new ServiceCollection()
			.AddLogging(l => l.ClearProviders())
			.AddWowsReplayUnpacker()
			.BuildServiceProvider()
			.GetRequiredService<ReplayUnpackerFactory>();
	}

	
	/// <summary>
	/// Test parsing a working replay (Assets/good.wowsreplay)
	/// </summary>
	[
		Theory,
		InlineData("0.10.10.wowsreplay"),
		InlineData("0.10.11.wowsreplay"),
		InlineData("0.11.0.wowsreplay"),
		InlineData("0.11.2.wowsreplay"),
		InlineData("12.6.wowsreplay"),
		InlineData("12.7.wowsreplay"),
	]
	public void TestReplay_Pass(string replayPath)
	{
		UnpackedReplay replay = _factory.GetUnpacker().Unpack(LoadReplay(replayPath));
		Assert.NotNull(replay);
	}
	
	
	private MemoryStream LoadReplay(string replayPath)
	{
		using FileStream fs = File.OpenRead(Path.Join(_sampleFolder, replayPath));
		MemoryStream ms = new();
		fs.CopyTo(ms);
		ms.Position = 0;

		return ms;
	}
}