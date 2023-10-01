﻿// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.EntitySerializer;
using Nodsoft.WowsReplaysUnpack.ExtendedData;
using Nodsoft.WowsReplaysUnpack.ExtendedData.Models;
using Nodsoft.WowsReplaysUnpack.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

string samplePath = Path.Join(Directory.GetCurrentDirectory(), "../../../..", "Replay-Samples");
FileStream _GetReplayFile(string name) => File.OpenRead(Path.Join(samplePath, name));

ServiceProvider? services = new ServiceCollection()
	.AddWowsReplayUnpacker(builder =>
	{
		//builder.AddReplayController<CVECheckOnlyController>();
		builder.AddExtendedData();
		builder.WithDefinitionStore<LocalDefinitionStore>();
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
//ExtendedDataReplay? unpackedReplay = (ExtendedDataReplay)replayUnpacker.GetExtendedDataUnpacker().Unpack(_GetReplayFile("good.wowsreplay"));

//foreach (ReplayMessage msg in replay.ChatMessages)
//{
//	Console.WriteLine($"[{GetGroupString(msg)}] {msg.EntityId} : {msg.MessageContent}");
//}



var alphaReplay = replayUnpacker.GetUnpacker().Unpack(_GetReplayFile("press_account_alpha.wowsreplay"));
var bravoReplay = replayUnpacker.GetUnpacker().Unpack(_GetReplayFile("unfinished_replay.wowsreplay"));

var alphaState = alphaReplay.Entities.Single(e => e.Value.Name == "BattleLogic").Value.ClientProperties
	.GetAsDict("state")
	.GetAsDict("missions")
	.GetAsArr("teamsScore");

var bravoState = bravoReplay.Entities.Single(e => e.Value.Name == "BattleLogic").Value.ClientProperties
	.GetAsDict("state")
	.GetAsDict("missions")
	.GetAsArr("teamsScore");

var scoreA = alphaState.GetAsDict(0).GetAsValue<ushort>("score");
var scoreB = alphaState.GetAsDict(1).GetAsValue<ushort>("score");

var _scoreA = bravoState.GetAsDict(0).GetAsValue<ushort>("score");
var _scoreB = bravoState.GetAsDict(1).GetAsValue<ushort>("score");



var test = alphaReplay.SerializeEntity<BattleLogic>("BattleLogic");

Console.WriteLine();
Console.ReadKey();

public class BattleLogic
{
	public Statee State { get; set; }
	public class Statee
	{
		[DataMember(Name = "missions")]
		public Missions _missions { get; set; }
		public class Missions
		{
			public List<TeamsScore> teamsScore { get; set; }
			public class TeamsScore
			{
				public ushort score { get; set; }
			}
		}
	}
}
public static class ext
{
	public static FixedDictionary GetAsDict(this Dictionary<string, object?> dict, string key) => dict[key] as FixedDictionary;
	public static FixedList GetAsArr(this Dictionary<string, object?> dict, string key) => dict[key] as FixedList;
	public static FixedDictionary GetAsDict(this FixedList list, int index) => list[index] as FixedDictionary;
	public static T GetAsValue<T>(this FixedDictionary dict, string key) => (T)dict[key];
}

//static string GetGroupString(ReplayMessage msg) => msg.MessageGroup switch
//{
//	"battle_team" => "Team",
//	"battle_common" => "All",
//	_ => ""
//};