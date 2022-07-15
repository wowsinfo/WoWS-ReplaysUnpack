// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack;
using Nodsoft.WowsReplaysUnpack.Services;
using System;
using System.IO;

string samplePath = Path.Join(Directory.GetCurrentDirectory(), "../../../..", "Replay-Samples");
FileStream GetReplayFile(string name)
	=> File.OpenRead(Path.Join(samplePath, name));

var services = new ServiceCollection()
	.AddWowsReplayUnpacker(builder =>
	{
		//builder.AddReplayController<TestCon>();
	})
	.AddLogging(logging =>
	{
		logging.ClearProviders();
		logging.AddConsole();
		logging.SetMinimumLevel(LogLevel.Information);
	})
	.BuildServiceProvider();

var replayUnpacker = services.GetRequiredService<ReplayUnpackerFactory>();

var unpackedReplay = replayUnpacker.GetUnpacker().Unpack(GetReplayFile("good.wowsreplay"));


//foreach (ReplayMessage msg in replay.ChatMessages)
//{
//	Console.WriteLine($"[{GetGroupString(msg)}] {msg.EntityId} : {msg.MessageContent}");
//}

Console.ReadKey();



//static string GetGroupString(ReplayMessage msg) => msg.MessageGroup switch
//{
//	"battle_team" => "Team",
//	"battle_common" => "All",
//	_ => ""
//};