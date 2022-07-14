using Nodsoft.WowsReplaysUnpack.Core.Exceptions;
using System.Text;
using System.Text.RegularExpressions;

namespace Nodsoft.WowsReplaysUnpack.Core.Security;

internal static class CVEChecks
{
	private static readonly Regex CVE_2022_31265_Regex = new(@"cnt\ssystem|commands", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

	public static void ScanForCVE_2022_31265(byte[] data, string? propertyOrArgumentName)
	{
		/*
		* Scan the UTF-8 blobs for any RCE injections.
		*
		* These can be picked up on Windows (98% of all users),
		* using the special moniker "cnt system", which defines an entry point for the RCE.
		* This moniker consists of detecting the "c" pickle opcode, followed by the "nt system" value.
		*/

		if (CVE_2022_31265_Regex.IsMatch(Encoding.UTF8.GetString(data)))
		{
			throw new CVESecurityException("CVE-2022-31265", data, propertyOrArgumentName);
		}
	}
}
