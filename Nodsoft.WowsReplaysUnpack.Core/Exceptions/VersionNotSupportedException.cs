namespace Nodsoft.WowsReplaysUnpack.Core.Exceptions;

public class VersionNotSupportedException : Exception
{
	public Version OldestVersion { get; }
	public Version RequestedVersion { get; }
	public VersionNotSupportedException(Version oldestVersion, Version requestedVersion) => (OldestVersion, RequestedVersion) = (oldestVersion, requestedVersion);
}
