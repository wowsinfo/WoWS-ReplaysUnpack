using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodsoft.WowsReplaysUnpack.Core.Exceptions;

public class VersionNotSupportedException : Exception
{
	public Version OldestVersion { get; }
	public Version RequestedVersion { get; }
	public VersionNotSupportedException(Version oldestVersion, Version requestedVersion)
	{
		OldestVersion = oldestVersion;
		RequestedVersion = requestedVersion;
	}
}
