using System.Diagnostics.CodeAnalysis;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

[Flags]
[SuppressMessage("ReSharper", "InconsistentNaming")] // Member Naming depends on strings defined in upstream definition files.
public enum EntityFlag
{
	CELL_PRIVATE = 1,
	CELL_PUBLIC = 2,
	OTHER_CLIENTS = 4,
	OWN_CLIENT = 8,
	BASE = 16,
	BASE_AND_CLIENT = 32,
	CELL_PUBLIC_AND_OWN = 64,
	ALL_CLIENTS = 128,
	EDITOR_ONLY = 256
}
