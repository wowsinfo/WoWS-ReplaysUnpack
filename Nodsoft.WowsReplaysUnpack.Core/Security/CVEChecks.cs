using Nodsoft.WowsReplaysUnpack.Core.Exceptions;
using System.Text;
using System.Text.RegularExpressions;

namespace Nodsoft.WowsReplaysUnpack.Core.Security;

/// <summary>
/// Provides scanning methods for CVEs.
/// </summary>
public static class CveChecks
{
	private static readonly Regex CVE_2022_31265_Regex = new(@"cnt\ssystem|commands", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

	/// <summary>
	/// Scans a buffer for <a href="https://www.cve.org/CVERecord?id=CVE-2022-31265">CVE-2022-31265</a> attack patterns.
	/// </summary>
	/// <param name="data">The buffer to scan.</param>
	/// <param name="propertyOrArgumentName">The name of the property or argument that is being scanned.</param>
	/// <exception cref="CveSecurityException">Thrown if the buffer contains a known CVE-2022-31265 attack pattern.</exception>
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
			throw new CveSecurityException("CVE-2022-31265", data, propertyOrArgumentName);
		}
	}
}
