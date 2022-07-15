using Razorvine.Pickle.Objects;
using System.Collections;
using System.Reflection;
using System.Text;

namespace Nodsoft.WowsReplaysUnpack.ExtendedData.Models;

public class ReplayPlayer
{
	public static PropertyInfo[] PropertyInfos = typeof(ReplayPlayer).GetProperties();
	public uint AccountId { get; set; }

	public bool AntiAbuseEnabled { get; set; }

	public uint AvatarId { get; set; }

	public string? CamouflageInfo { get; set; }

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

	public Lazy<ShipData?> ShipData { get; }

	public ReplayPlayer(ShipConfigMapping shipConfigMapping)
	{
		ShipData = new(() =>
		{
			if (string.IsNullOrEmpty(ShipConfigDump))
				return null;

			byte[] byteArray = Encoding.Latin1.GetBytes(ShipConfigDump);
			using MemoryStream memoryStream = new(byteArray);
			using BinaryReader binaryReader = new(memoryStream);

			List<uint> configDumpList = new();

			while (memoryStream.Position != memoryStream.Length)
				configDumpList.Add(binaryReader.ReadUInt32());
			var x = ProcessShipConfigDumpX(configDumpList, shipConfigMapping);
			var y= ProcessShipConfigDump(configDumpList);
			return new(ProcessShipConfigDump(configDumpList), shipConfigMapping);
		});
	}

	private static IReadOnlyList<uint[]> ProcessShipConfigDump(IReadOnlyList<uint> rawConfigDump)
	{
		// First value is useless, then it's shipId, then total values
		// After that it's the same format until the end
		// Length Indicator + Items (As many as the length indicator defines) 
		uint[] tempList = rawConfigDump.Skip(3).ToArray();
		List<uint[]> resultList = new()
		{
			new[] { rawConfigDump[1] }, // ShipId
			new[] { rawConfigDump[2] }  // TotalValues
		};
		while (tempList.Length > 0)
		{
			int length = (int)tempList[0];
			tempList = tempList.Skip(1).ToArray();
			resultList.Add(tempList.Take(length).ToArray());
	    tempList = tempList.Skip(length).ToArray();
		}

		return resultList;
	}

	private static IReadOnlyList<IReadOnlyList<uint>> ProcessShipConfigDumpX(IReadOnlyList<uint> rawConfigDump, ShipConfigMapping configMapping)
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
				if (step == configMapping.TotalValueCount || step == configMapping.AutoSupplyState)
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
}
