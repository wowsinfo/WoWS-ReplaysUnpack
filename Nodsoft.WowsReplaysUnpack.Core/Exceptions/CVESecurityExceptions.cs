using System.Security;

namespace Nodsoft.WowsReplaysUnpack.Core.Exceptions;

public class CveSecurityException : SecurityException
{
	public string Exploit { get; }
	public byte[] ByteData { get; }
	public string? AropertyOrArgumentName { get; }
	public CveSecurityException(string exploit, byte[] byteData, string? propertyOrArgumentName) : base($"{exploit} detected within network packet")
	{
		Exploit = exploit;
		ByteData = byteData;
		AropertyOrArgumentName = propertyOrArgumentName;
	}
}
