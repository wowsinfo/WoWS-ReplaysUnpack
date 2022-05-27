using Nodsoft.WowsReplaysUnpack.Data;
using System;
using System.IO;
using System.Security;
using Xunit;

namespace Nodsoft.WowsReplaysUnpack.Tests;

public class ReplaySanitizerTests
{
	private readonly ReplayUnpacker _unpacker = new();
	
	/// <summary>
	/// Test parsing a working replay (Assets/good.wowsreplay)
	/// </summary>
	[Fact]
	public void TestGoodReplay_Pass()
	{
		ReplayRaw replayRaw = _unpacker.UnpackReplay(LoadReplay(Path.Join(Directory.GetCurrentDirectory(), "Samples", "good.wowsreplay")));
		Assert.NotNull(replayRaw);
	}
	
	private MemoryStream LoadReplay(string replayPaths)
	{
		using FileStream fs = File.OpenRead(replayPaths);
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
		Assert.Throws<SecurityException>(() => _unpacker.UnpackReplay(LoadReplay(Path.Join(Directory.GetCurrentDirectory(), "Samples", "payload.wowsreplay"))));
	}
}