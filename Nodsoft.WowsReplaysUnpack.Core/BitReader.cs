namespace Nodsoft.WowsReplaysUnpack.Core;

public class BitReader : BinaryReader
{
	private byte readBits = 0;
	private List<short> bitsCache = new List<short>();

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
			foreach (var i in Enumerable.Range(0, 8).Reverse())
				yield return (short)((value >> i) & 1);
	}

	public byte ReadNextByte() => ReadByte();

	public short ReadNextBit()
	{
		if (bitsCache.Count == 0)
		{
			var nextByte = ReadNextByte();
			bitsCache = StringBits(nextByte).ToList();
		}
		readBits += 1;

		var returnValue = bitsCache[0];
		bitsCache.RemoveAt(0);
		return returnValue;
	}

	public short ReadBits(int bitsCount)
	{
		if (bitsCount == 0)
			return 0;
		short value = 0;
		while (bitsCount > 0)
		{
			var bit = ReadNextBit();
			value = (short)((value << 1) | bit);
			bitsCount -= 1;
		}
		return value;
	}
}
