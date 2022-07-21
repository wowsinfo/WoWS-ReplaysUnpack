namespace Nodsoft.WowsReplaysUnpack.Core;

public class BitReader : BinaryReader
{
	//private byte readBits;
	private List<short> bitsCache = new();

	// public byte BytesRead => (byte)Math.Ceiling(readBits / 8d);
	public BitReader(Stream input) : base(input) { }

	public static int BitsRequired(int length) => length < 1 ? 0 : (int)Math.Ceiling(Math.Log2(length));

	public byte[] ReadRest() => ReadBytes((int)BaseStream.Length - (int)BaseStream.Position);

	private static IEnumerable<short> StringBits(byte value)
	{
		return Enumerable.Range(0, 8).Reverse().Select(i => (short)((value >> i) & 1));
	}

	private byte ReadNextByte() => ReadByte();

	private int ReadNextBit()
	{
		if (bitsCache.Count is 0)
		{
			byte nextByte = ReadNextByte();
			bitsCache = StringBits(nextByte).ToList();
		}

		// readBits += 1;

		short returnValue = bitsCache[0];
		bitsCache.RemoveAt(0);

		return returnValue;
	}

	public int ReadBits(int bitsCount)
	{
		if (bitsCount is 0)
		{
			return 0;
		}

		int value = 0;

		while (bitsCount > 0)
		{
			int bit = ReadNextBit();
			value = (value << 1) | bit;
			bitsCount -= 1;
		}

		return value;
	}
}