using System.Reflection;

namespace Nodsoft.WowsReplaysUnpack.Core.Network;

/// <summary>
/// Type definitions and utilities for working with network packets.
/// </summary>
public static class NetworkPacketTypes
{
	public const uint BasePlayerCreate = 0x0;
	public const uint CellPlayerCreate = 0x1;
	public const uint EntityControl = 0x2;
	public const uint EntityEnter = 0x3;
	public const uint EntityLeave = 0x4;
	public const uint EntityCreate = 0x5;
	public const uint EntityProperty = 0x7;
	public const uint EntityMethod = 0x8;
	public const uint Map = 0x27;
	public const uint NestedProperty = 0x22;
	public const uint Position = 0x0a;
	public const uint PlayerPosition = 0x2b;

	private static readonly Dictionary<uint, string> _names = new();

	static NetworkPacketTypes()
	{
		foreach (FieldInfo field in typeof(NetworkPacketTypes).GetFields(BindingFlags.Public))
		{
			_names.Add((uint)field.GetValue(null)!, field.Name);
		}
	}

	/// <summary>
	/// Gets the name of a network packet type by its ID.
	/// </summary>
	/// <param name="id">The ID of the packet type.</param>
	/// <returns>The name of the packet type.</returns>
	public static string GetTypeName(uint id) => _names.ContainsKey(id) ? _names[id] : $"Unsupported Type ({id})";
}
