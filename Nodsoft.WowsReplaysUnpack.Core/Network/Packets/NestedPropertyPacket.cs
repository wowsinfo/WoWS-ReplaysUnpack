using Nodsoft.WowsReplaysUnpack.Core.Entities;
using Nodsoft.WowsReplaysUnpack.Core.Models;

namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

public class NestedPropertyPacket : NetworkPacketBase
{
	public uint EntityId { get; }
	public bool IsSlice { get; }
	public byte DataSize { get; }
	public byte[] Data { get; }

	public NestedPropertyPacket(int packetIndex, BinaryReader binaryReader) : base(packetIndex)
	{
		EntityId = binaryReader.ReadUInt32();
		IsSlice = binaryReader.ReadBoolean();
		DataSize = binaryReader.ReadByte();
		_ = binaryReader.ReadBytes(3); // unknown
		Data = binaryReader.ReadBytes(DataSize);
	}

	public void Apply(Entity entity)
	{
		object? obj = entity;
		using BitReader bitReader = new(new MemoryStream(Data));

		while (bitReader.ReadBits(1) > 0 && obj is not null)
		{
			int length = obj switch
			{
				Entity e => e.ClientProperties.Count,
				IFixedLength f => f.Length,
				_ => 0
			};

			int maxBits = BitReader.BitsRequired(length);
			int propertyIndex = bitReader.ReadBits(maxBits);

			obj = obj switch
			{
				FixedDictionary d => d.ElementValueAt(propertyIndex),
				FixedList l => l.ElementAt(propertyIndex),
				Entity e when e.GetClientPropertyNameForIndex(propertyIndex) is { } field => e.ClientProperties[field],
				_ => null
			};
		}

		if (obj is FixedDictionary dict)
		{
			int maxBits = BitReader.BitsRequired(dict.Count);
			int index = bitReader.ReadBits(maxBits);

			string fieldName = dict.ElementAt(index).Key;
			DataTypes.DataTypeBase fieldType = dict.GetDataTypeForIndex(index);
			using BinaryReader fieldValueReader = new(new MemoryStream(bitReader.ReadRest()));
			dict[fieldName] = fieldType.GetValue(fieldValueReader, null);
		}

		if (obj is FixedList list)
		{
			int maxBits = BitReader.BitsRequired(IsSlice ? list.Length + 1 : list.Length);

			int index = bitReader.ReadBits(maxBits);
			int endIndex = IsSlice ? bitReader.ReadBits(maxBits) : 0;

			byte[] data = bitReader.ReadRest();

			if (data.Length <= 0)
			{
				if (IsSlice)
				{
					list.Slice(index, endIndex, Array.Empty<object?>());
				}
				else
				{
					list[index] = null;
				}

				return;
			}

			List<object?> newElementValues = new();
			using BinaryReader elementsReader = new(new MemoryStream(data));

			while (elementsReader.BaseStream.Position < elementsReader.BaseStream.Length)
			{
				object? xValue = list.ElementType.GetValue(elementsReader, null);
				newElementValues.Add(xValue);
			}


			if (IsSlice)
			{
				list.Slice(index, endIndex, newElementValues);
			}
			else
			{
				list[index] = newElementValues[0];
			}
		}
	}
}