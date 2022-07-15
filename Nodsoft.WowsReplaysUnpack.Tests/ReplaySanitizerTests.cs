using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Exceptions;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Services;
using System.IO;
using Xunit;

namespace Nodsoft.WowsReplaysUnpack.Tests;

public class ReplaySanitizerTests
{
	private readonly ReplayUnpackerFactory _factory;
	private readonly string _sampleFolder = Path.Join(Directory.GetCurrentDirectory(), "../../../..", "Replay-Samples");

	public ReplaySanitizerTests()
	{
		var serviceCollection = new ServiceCollection();
		serviceCollection.AddLogging(l => l.ClearProviders());
		serviceCollection.AddWowsReplayUnpacker();
		var services = serviceCollection.BuildServiceProvider();
		_factory = services.GetRequiredService<ReplayUnpackerFactory>();
	}

	/// <summary>
	/// Test parsing a working replay (Assets/good.wowsreplay)
	/// </summary>
	[Fact]
	public void TestGoodReplay_Pass()
	{
		UnpackedReplay replay = _factory.GetUnpacker().Unpack(LoadReplay("good.wowsreplay"));
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

	/// <summary>
	/// Test malicious replay detection (Assets/payload.wowsreplay)
	/// </summary>
	[Fact]
	public void TestPayloadReplayDetection()
	{
		Assert.Throws<CVESecurityException>(() => _factory.GetUnpacker().Unpack(LoadReplay("payload.wowsreplay")));
	}
}