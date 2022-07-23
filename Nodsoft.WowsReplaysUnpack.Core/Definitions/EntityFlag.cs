using System.Diagnostics.CodeAnalysis;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

/// <summary>
/// Defines Entity flags found in definition files.
/// </summary>
[Flags]
[SuppressMessage("ReSharper", "InconsistentNaming")] // Member Naming depends on strings defined in upstream definition files.
public enum EntityFlag
{
	CELL_PRIVATE = 0x001,
	CELL_PUBLIC = 0x002,
	OTHER_CLIENTS = 0x004,
	OWN_CLIENT = 0x008,
	BASE = 0x010,
	BASE_AND_CLIENT = 0x020,
	CELL_PUBLIC_AND_OWN = 0x040,
	ALL_CLIENTS = 0x080,
	EDITOR_ONLY = 0x100
}
