namespace Nodsoft.WowsReplaysUnpack.Core;

public class BitReader : BinaryReader
{
	private byte readBits = 0;
	private List<short> bitsCache = new();

	public byte BytesRead => (byte)Math.Ceiling(readBits / 8d);
	public BitReader(Stream input) : base(input)
	{
	}

	public static int BitsRequired(int length)
	{
		if (length < 1)
			return 0;
		return (int)Math.Ceiling(Math.Log2(length));
	}

	public byte[] ReadRest() => ReadBytes((int)BaseStream.Length - (int)BaseStream.Position);

	public static IEnumerable<short> StringBits(byte value)
	{
			foreach (int i in Enumerable.Range(0, 8).Reverse())
				yield return (short)((value >> i) & 1);
	}

	public byte ReadNextByte() => ReadByte();

	public int ReadNextBit()
	{
		if (bitsCache.Count == 0)
		{
			byte nextByte = ReadNextByte();
			bitsCache = StringBits(nextByte).ToList();
		}
		readBits += 1;

		short returnValue = bitsCache[0];
		bitsCache.RemoveAt(0);
		return returnValue;
	}

	public int ReadBits(int bitsCount)
	{
		if (bitsCount == 0)
			return 0;
		int value = 0;
		while (bitsCount > 0)
		{
			int bit = ReadNextBit();
			value = ((value << 1) | bit);
			bitsCount -= 1;
		}
		return value;
	}
}
