namespace Nodsoft.WowsReplaysUnpack.Core.Models;

/// <summary>
/// Specifies a fixed-length object.
/// </summary>
/// <remarks>
///	Implementers are responsible for ensuring the fixed nature of an object's length.
/// </remarks>
internal interface IFixedLength
{
	/// <summary>
	/// Gets the length of the fixed-length object
	/// </summary>
	int Length { get; }
}