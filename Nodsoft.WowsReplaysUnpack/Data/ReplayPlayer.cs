using Razorvine.Pickle.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Nodsoft.WowsReplaysUnpack.Data;

public sealed record ReplayPlayer
{
	public ReplayPlayer()
	{
		ShipData = new(() =>
		{
			string chunkedConfigDump = (string)Properties[nameof(ShipConfigDump)];
			byte[] byteArray = Encoding.Latin1.GetBytes(chunkedConfigDump);
			MemoryStream memoryStream = new(byteArray);
			List<uint> configDumpList = new();

			while (memoryStream.Position != memoryStream.Length)
			{
				byte[] byteData = new byte[4];
				memoryStream.Read(byteData);
				configDumpList.Add(BitConverter.ToUInt32(byteData));
			}

			return new(ProcessShipConfigDump(configDumpList));
		});
	}

	private static IReadOnlyList<IReadOnlyList<uint>> ProcessShipConfigDump(IReadOnlyList<uint> rawConfigDump)
	{
		int listPosition = 0;
		int currentLength = (int)rawConfigDump[listPosition++];
		List<List<uint>> groupedList = new();
		List<uint> tmpList = new();
		int step = 0;
		while (listPosition < rawConfigDump.Count)
		{
			if (currentLength == 0)
			{
				groupedList.Add(tmpList);
				step++;
				if (step is Constants.ShipConfigMapping.TotalValueCount or Constants.ShipConfigMapping.AutoSupplyState)
				{
					tmpList = new() { rawConfigDump[listPosition++] };
				}
				else
				{
					currentLength = (int)rawConfigDump[listPosition++];
					tmpList = new();
				}
			}
			else
			{
				tmpList.Add(rawConfigDump[listPosition++]);
				currentLength--;
			}
		}
			
		groupedList.Add(tmpList);
		return groupedList;
	}

	public IReadOnlyDictionary<string, object> Properties { get; init; } = new Dictionary<string, object>();

	public uint AccountId { get; set; }

	public bool AntiAbuseEnabled { get; set; }

	public uint AvatarId { get; set; }

	public object? CamouflageInfo { get; set; }

	public uint ClanColor { get; set; }

	public uint ClanId { get; set; }

	public string? ClanTag { get; set; }

	public ArrayList? CrewParams { get; set; }

	public ArrayList? DogTag { get; set; }

	public short FragsCount { get; set; }

	public bool FriendlyFireEnabled { get; set; }

	public uint Id { get; set; }

	public bool InvitationsEnabled { get; set; }

	public bool IsAbuser { get; set; }

	public bool IsAlive { get; set; }

	public bool IsBot { get; set; }

	public bool IsClientLoaded { get; set; }

	public bool IsConnected { get; set; }

	public bool IsHidden { get; set; }

	public bool IsLeaver { get; set; }

	public bool IsPreBattleOwner { get; set; }

	public bool IsTShooter { get; set; }

	public short KilledBuildingsCount { get; set; }

	public uint MaxHealth { get; set; }

	public string Name { get; set; } = string.Empty;

	public ClassDict? PlayerMode { get; set; }

	public uint PreBattleIdOnStart { get; set; }

	public object? PreBattleSign { get; set; }

	public uint PreBattleId { get; set; }

	public string Realm { get; set; } = string.Empty;

	public Hashtable? ShipComponents { get; set; }

	public string ShipConfigDump { get; set; } = string.Empty;

	public uint ShipId { get; set; }

	public uint ShipParamsId { get; set; }

	public uint SkinId { get; set; }

	public uint TeamId { get; set; }

	public bool TtkStatus { get; set; }

	public Lazy<ShipData> ShipData { get; }
}