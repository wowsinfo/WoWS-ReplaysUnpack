using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Exceptions;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Services;
using Xunit;

namespace Nodsoft.WowsReplaysUnpack.Tests;

public class ReplaySanitizerTests
{
	private readonly ReplayUnpackerFactory _factory;
	private readonly string _sampleFolder = Path.Join(Directory.GetCurrentDirectory(), "../../../..", "Replay-Samples");

	public ReplaySanitizerTests()
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
		Assert.Throws<CveSecurityException>(() => _factory.GetUnpacker().Unpack(LoadReplay("payload.wowsreplay")));
	}
}