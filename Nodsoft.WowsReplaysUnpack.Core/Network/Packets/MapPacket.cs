using System.Text;

namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

public class MapPacket : ANetworkPacket
{
	public int SpaceId { get; }
	public long ArenaId { get; }
	public string Name { get; }

	public MapPacket(int packetIndex, BinaryReader binaryReader) : base(packetIndex)
	{
		SpaceId = binaryReader.ReadInt32();
		ArenaId = binaryReader.ReadInt64();

		int nameSize = binaryReader.ReadInt32();

		// I assume this is some useless stuff about the map that we have to skip
		Stream stream = binaryReader.BaseStream;
		if (stream.Position + nameSize + 16 * 4 != stream.Length)
		{
			_ = binaryReader.ReadBytes(16 * 8 + 4);
			nameSize = binaryReader.ReadInt32();
		}
		Name = Encoding.UTF8.GetString(binaryReader.ReadBytes(nameSize));
	}
}
