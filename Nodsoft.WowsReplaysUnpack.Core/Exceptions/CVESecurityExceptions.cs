using System.Security;

namespace Nodsoft.WowsReplaysUnpack.Core.Exceptions;

/// <summary>
/// Defines an exception thrown when a security vulnerability is detected within a replay file.
/// </summary>
public class CveSecurityException : SecurityException
{
	/// <summary>
	/// CVE ID defining the vulnerability.
	/// </summary>
	public string Exploit { get; }

	/// <summary>
	/// Buffer containing the relevant data from the replay file.
	/// </summary>
	public byte[] ByteData { get; }

	/// <summary>
	/// Property or argument name where the vulnerability was detected.
	/// </summary>
	public string? PropertyOrArgumentName { get; }


	public CveSecurityException(string exploit, byte[] byteData, string? propertyOrArgumentName) : base($"{exploit} detected within network packet")
		=> (Exploit, ByteData, PropertyOrArgumentName) = (exploit, byteData, propertyOrArgumentName);
}