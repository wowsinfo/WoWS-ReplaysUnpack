// See https://aka.ms/new-console-template for more information
using Nodsoft.WowsReplaysUnpack;
using Nodsoft.WowsReplaysUnpack.Data;

Console.WriteLine();


using FileStream fs = File.OpenRead(Path.Join(Directory.GetCurrentDirectory(), "10.10.wowsreplay"));
ReplayRaw replay = new ReplayUnpacker().UnpackReplay(fs);

foreach (ReplayMessage msg in replay.ChatMessages)
{
	Console.WriteLine($"[{GetGroupString(msg)}] {msg.EntityId} : {msg.MessageContent}");
}

Console.ReadKey();


static string GetGroupString(ReplayMessage msg) => msg.MessageGroup switch
{
	"battle_team" => "Team",
	"battle_common" => "All"
};