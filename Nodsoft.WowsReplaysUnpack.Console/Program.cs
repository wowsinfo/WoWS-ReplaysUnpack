// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Nodsoft.WowsReplaysUnpack.Console;
using Nodsoft.WowsReplaysUnpack.Data;


Console.WriteLine();

/*
BenchmarkRunner.Run<UnpackBenchmark>(DefaultConfig.Instance
	.WithOptions(ConfigOptions.DisableOptimizationsValidator)
);

/**/


ReplayRaw replay = new UnpackBenchmark().GetReplay();

/**/


foreach (ReplayMessage msg in replay.ChatMessages)
{
	Console.WriteLine($"[{GetGroupString(msg)}] {msg.EntityId} : {msg.MessageContent}");
}

/**/

Console.ReadKey();


static string GetGroupString(ReplayMessage msg) => msg.MessageGroup switch
{
	"battle_team" => "Team",
	"battle_common" => "All",
	_ => ""
};

/**/