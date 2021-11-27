// See https://aka.ms/new-console-template for more information
using Nodsoft.WowsReplaysUnpack;

Console.WriteLine("Hello, World!");


using (FileStream fs = File.OpenRead(Path.Join(Directory.GetCurrentDirectory(), "10.10.wowsreplay")))
{
	new ReplayUnpacker().UnpackReplay(fs);
}

Console.ReadKey();