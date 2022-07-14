using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

/// <summary>
/// Fires when servers requests client to change
/// entity's property with some arguments
/// </summary>
public class EntityPropertyPacket : INetworkPacket
{
	public int EntityId { get; }
	public int MessageId { get; }
	public byte[] Data { get; }

	public EntityPropertyPacket(BinaryReader binaryReader)
	{
		EntityId = binaryReader.ReadInt32();
		MessageId = binaryReader.ReadInt32();
		Data = binaryReader.ReadBytesWithHeader();
	}

}
