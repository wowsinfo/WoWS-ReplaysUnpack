using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Nodsoft.WowsReplaysUnpack.Core.Exceptions;

public class CVESecurityException: SecurityException
{
	public string Exploit { get; }
	public byte[] PacketData { get; }
	public uint PacketType { get; }
	public float PacketTime { get; }

	public CVESecurityException(string exploit, 
		byte[] packetData, uint packetType, float packetTime): base($"{exploit} detected within network packet")
	{
		Exploit = exploit;
		PacketData = packetData;
		PacketType = packetType;
		PacketTime = packetTime;
	}
}
