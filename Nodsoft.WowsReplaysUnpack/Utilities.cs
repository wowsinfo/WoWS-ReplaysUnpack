namespace Nodsoft.WowsReplaysUnpack;


public class Utilities
{
	internal static IEnumerable<byte[]> ChunkData(Stream data, int len = 8)
	{
		while (data.Position < data.Length)
		{
			byte[] chunk = new byte[len];
			data.Read(chunk);
			yield return chunk;
		}
	}
}
