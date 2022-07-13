namespace Nodsoft.WowsReplaysUnpack.Models;

public enum ReplayUnpackerMode
{
	Default = 0,
	IgnoreCVECheck = 1,
	OnlyCVECheck = 2
}

public class ReplayUnpackerOptions
{
	public ReplayUnpackerMode Mode { get; set; } = ReplayUnpackerMode.Default;
}
