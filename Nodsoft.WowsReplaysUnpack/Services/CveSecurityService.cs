using Nodsoft.WowsReplaysUnpack.Core.Exceptions;
using System.Text;
using System.Text.RegularExpressions;

namespace Nodsoft.WowsReplaysUnpack.Services;

public class CveSecurityService
{
	private static readonly Regex CVE_2022_31265_Regex = new(@"cnt\ssystem|commands", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

	public void ScanForVulnerabilities(byte[] packetData, uint packetType, float packetTime)
	{
		ScanForCVE_2022_31265(packetData, packetType, packetTime);
	}

	/// <summary>
	/// Provides detection for CVE-2022-31265 RCE injections. <br />
	/// See : https://www.cve.org/CVERecord?id=CVE-2022-31265
	/// </summary>
	/// <param name="blob">Python pickle byte array to scan.</param>
	/// <param name="blobName">Name of byte array (used for exception source reporting)</param>
	/// <exception cref="CVESecurityException">Raised when a CVE-2022-31265 RCE Injection attack is detected within a blob.</exception>
	private void ScanForCVE_2022_31265(byte[] data, uint packetType, float packetTime)
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
			throw new CVESecurityException("CVE-2022-31265", data, packetType, packetTime);
		}
	}
}
