using Nodsoft.WowsReplaysUnpack.Core.Entities;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using System.Collections.ObjectModel;

namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

public class NestedPropertyPacket : ANetworkPacket
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
			var length = 0;
			if (obj is Entity _e)
				length = _e.ClientProperties.Count;
			else if (obj is IFixedLength fl)
				length = fl.Length;

			var maxBits = BitReader.BitsRequired(length);
			var propertyIndex = bitReader.ReadBits(maxBits);
			if (obj is FixedDictionary __d)
				obj = __d.ElementValueAt(propertyIndex);
			else if (obj is FixedList __a)
				obj = __a.ElementAt(propertyIndex);
			else if (obj is Entity __e)
			{
				var field = __e.GetClientPropertyNameForIndex(propertyIndex);
				obj = __e.ClientProperties[field];
			}
		}

		if (obj is FixedDictionary dict)
		{
			var maxBits = BitReader.BitsRequired(dict.Count);
			var index = bitReader.ReadBits(maxBits);

			var fieldName = dict.ElementAt(index).Key;
			var fieldType = dict.GetDataTypeForIndex(index);
			using BinaryReader fieldValueReader = new(new MemoryStream(bitReader.ReadRest()));
			dict[fieldName] = fieldType.GetValue(fieldValueReader, null);
		}
		if (obj is FixedList list)
		{
			var maxBits = 0;
			if (IsSlice)
				maxBits = BitReader.BitsRequired(list.Length + 1);
			else
				maxBits = BitReader.BitsRequired(list.Length);

			var index = bitReader.ReadBits(maxBits);
			var endIndex = IsSlice ? bitReader.ReadBits(maxBits) : 0;

			var data = bitReader.ReadRest();
			if (data.Length <= 0)
			{
				if (IsSlice)
					list.Slice(index, endIndex, Array.Empty<object?>());
				else
					list[index] = null;
				return;
			}

			var newElementValues = new List<object?>();
			using BinaryReader elementsReader = new(new MemoryStream(data));
			while (elementsReader.BaseStream.Position < elementsReader.BaseStream.Length)
			{
				var xValue = list.ElementType.GetValue(elementsReader, null);
				newElementValues.Add(xValue);
			}


			if (IsSlice)
				list.Slice(index, endIndex, newElementValues);
			else
				list[index] = newElementValues[0];
		}
	}
}
