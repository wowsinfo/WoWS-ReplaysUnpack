namespace Nodsoft.WowsReplaysUnpack.Core.Exceptions;

/// <summary>
/// Thrown when a replay's game client version is not supported by the unpacker.
/// </summary>
public sealed class VersionNotSupportedException : ApplicationException
{
	/// <summary>
	/// Oldest supported game client version.
	/// </summary>
	public Version OldestVersion { get; }

	/// <summary>
	/// Unsupported game client version of the replay.
	/// </summary>
	/// <remarks>
	///	This version will usually be older than <see cref="OldestVersion"/>.
	/// </remarks>
	public Version RequestedVersion { get; }

	public VersionNotSupportedException(Version oldestVersion, Version requestedVersion) => (OldestVersion, RequestedVersion) = (oldestVersion, requestedVersion);
}