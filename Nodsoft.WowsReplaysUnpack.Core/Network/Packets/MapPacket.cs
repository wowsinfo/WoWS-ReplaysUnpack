using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

public class MapPacket : INetworkPacket
{
	public int SpaceId { get; }
	public long ArenaId { get; }
	public string Name { get; }

	public MapPacket(BinaryReader binaryReader)
	{
		SpaceId = binaryReader.ReadInt32();
		ArenaId = binaryReader.ReadInt64();

		var nameSize = binaryReader.ReadInt32();

		// I assume this is some useless stuff about the map that we have to skip
		var stream = binaryReader.BaseStream;
		if (stream.Position + nameSize + 16 * 4 != stream.Length)
		{
			_ = binaryReader.ReadBytes(16 * 8 + 4);
			nameSize = binaryReader.ReadInt32();
		}
		Name = Encoding.UTF8.GetString(binaryReader.ReadBytes(nameSize));
	}
}
