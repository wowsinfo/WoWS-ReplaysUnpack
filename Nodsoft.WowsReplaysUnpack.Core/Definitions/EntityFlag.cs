namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

[Flags]
public enum EntityFlag
{
	CELL_PRIVATE = 0,
	CELL_PUBLIC = 1,
	OTHER_CLIENTS = 2,
	OWN_CLIENT = 4,
	BASE = 8,
	BASE_AND_CLIENT = 16,
	CELL_PUBLIC_AND_OWN = 32,
	ALL_CLIENTS = 64,
	EDITOR_ONLY = 128
}
