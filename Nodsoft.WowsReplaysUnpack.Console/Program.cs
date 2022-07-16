// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack;
using Nodsoft.WowsReplaysUnpack.ExtendedData;
using Nodsoft.WowsReplaysUnpack.ExtendedData.Models;
using Nodsoft.WowsReplaysUnpack.Services;
using System;
using System.IO;

string samplePath = Path.Join(Directory.GetCurrentDirectory(), "../../../..", "Replay-Samples");
FileStream _GetReplayFile(string name) => File.OpenRead(Path.Join(samplePath, name));

ServiceProvider? services = new ServiceCollection()
	.AddWowsReplayUnpacker(builder =>
	{
		//builder.AddReplayController<CVECheckOnlyController>();
		builder.AddExtendedData();
	})
	.AddLogging(logging =>
	{
		logging.ClearProviders();
		logging.AddConsole();
		logging.SetMinimumLevel(LogLevel.Error);
	})
	.BuildServiceProvider();

ReplayUnpackerFactory? replayUnpacker = services.GetRequiredService<ReplayUnpackerFactory>();

//var unpackedReplay = replayUnpacker.GetUnpacker().Unpack(GetReplayFile("payload.wowsreplay"));
//var unpackedReplay = replayUnpacker.GetUnpacker<CVECheckOnlyController>().Unpack(GetReplayFile("payload.wowsreplay"));
ExtendedDataReplay? unpackedReplay = (ExtendedDataReplay)replayUnpacker.GetExtendedDataUnpacker().Unpack(_GetReplayFile("good.wowsreplay"));

//foreach (ReplayMessage msg in replay.ChatMessages)
//{
//	Console.WriteLine($"[{GetGroupString(msg)}] {msg.EntityId} : {msg.MessageContent}");
//}
Console.WriteLine("DONE");
Console.ReadKey();



//static string GetGroupString(ReplayMessage msg) => msg.MessageGroup switch
//{
//	"battle_team" => "Team",
//	"battle_common" => "All",
//	_ => ""
//};