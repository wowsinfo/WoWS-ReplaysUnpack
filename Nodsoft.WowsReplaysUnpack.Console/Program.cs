// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;
using Nodsoft.WowsReplaysUnpack.Data;
using Nodsoft.WowsReplaysUnpack.Services;
using System;
using System.IO;

Console.WriteLine();

var x = new byte[] { 20, 12, 45, 12 };
var s = BitConverter.ToInt32(x);


var serviceCollection = new ServiceCollection();
serviceCollection.AddReplayUnpacker();
serviceCollection.AddLogging(logging =>
{
	logging.ClearProviders();
	logging.AddConsole();
	logging.SetMinimumLevel(LogLevel.Information);
});
var services = serviceCollection.BuildServiceProvider();

var replayUnpacker = services.GetRequiredService<ReplayUnpackerService>();

FileStream fs = File.OpenRead(Path.Join(Directory.GetCurrentDirectory(), "sampleData", "0.11.2.wowsreplay"));
//FileStream fs = File.OpenRead(Path.Join(Directory.GetCurrentDirectory(), "../../../..", "Nodsoft.WowsReplaysUnpack.Tests", "Samples", "payload.wowsreplay"));
replayUnpacker.Unpack(fs);
//BenchmarkRunner.Run<UnpackBenchmark>(DefaultConfig.Instance
//	.WithOptions(ConfigOptions.DisableOptimizationsValidator)
//	.AddJob(Job.Default.WithRuntime(CoreRuntime.Core50))
//	.AddJob(Job.Default.WithRuntime(CoreRuntime.Core60))
//);

/**/

/*
ReplayRaw replay = new UnpackBenchmark().GetReplay();

/**/

/*
foreach (ReplayMessage msg in replay.ChatMessages)
{
	Console.WriteLine($"[{GetGroupString(msg)}] {msg.EntityId} : {msg.MessageContent}");
}

/**/
//var reader = new DefinitionsReader(typeof(ReplayUnpackerService).Assembly, new Version(0, 11, 5));
//var alias = new Alias(reader);
//var x = new EntityDefinition("Avatar.def", "entity_defs", reader, alias);
Console.ReadKey();


#pragma warning disable CS8321
static string GetGroupString(ReplayMessage msg) => msg.MessageGroup switch
{
	"battle_team" => "Team",
	"battle_common" => "All",
	_ => ""
};

public class TestReplay : UnpackedReplay
{
	public TestReplay(ArenaInfo arenaInfo) : base(arenaInfo)
	{
	}
}
public class TestReplayController : DefaultReplayController
{
	public new TestReplay? Replay { get; protected set; }

	public override UnpackedReplay CreateUnpackedReplay(ArenaInfo arenaInfo)
	{
		Replay = new TestReplay(arenaInfo);
		return Replay!;
	}
}

#pragma warning restore CS8321

/**/