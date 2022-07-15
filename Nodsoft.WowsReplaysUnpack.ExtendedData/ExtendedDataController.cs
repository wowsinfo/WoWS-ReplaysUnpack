using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Controllers;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Entities;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.ExtendedData.Models;
using Nodsoft.WowsReplaysUnpack.ExtendedData.VersionMappings;
using Razorvine.Pickle;
using System.Collections;
using System.Reflection;

namespace Nodsoft.WowsReplaysUnpack.ExtendedData
{
	public class ExtendedDataController : ExtendedDataController<ExtendedDataController>
	{
		public ExtendedDataController(VersionMappingFactory versionMappingFactory, IDefinitionStore definitionStore, ILogger<Entity> entityLogger)
			: base(versionMappingFactory, definitionStore, entityLogger)
		{
		}
	}

	public class ExtendedDataController<T> : AReplayControllerBase<T>
		where T : class, IReplayController
	{
		private readonly VersionMappingFactory _versionMappingFactory;

		static ExtendedDataController()
		{
			Unpickler.registerConstructor("CamouflageInfo", "CamouflageInfo", new CamouflageInfo());
		}
		public ExtendedDataController(VersionMappingFactory versionMappingFactory,
			IDefinitionStore definitionStore, ILogger<Entity> entityLogger)
			: base(definitionStore, entityLogger)
		{
			_versionMappingFactory = versionMappingFactory;
		}

		public ExtendedDataReplay ExtendedReplay => (ExtendedDataReplay)Replay!;
		public override UnpackedReplay CreateUnpackedReplay(ArenaInfo arenaInfo)
		{
			Replay = new ExtendedDataReplay(arenaInfo);
			return Replay;
		}

		[MethodSubscription("Avatar", "onChatMessage", includeEntity: false, includePacketTime: true)]
		public void OnChatMessage(float packetTime, int entityId, string messageGroup, string messageContent, string xx)
		{
			ExtendedReplay.ChatMessages.Add(new ChatMessage((uint)entityId, packetTime, messageGroup, messageContent));
		}

		[MethodSubscription("Avatar", "onArenaStateReceived", true, false)]
		public void OnArenaStateReceived(Dictionary<string, object?> arguments)
		{
			byte[]? playerStatesData = (byte[]?)arguments["playersStates"];
			if (playerStatesData is null)
				return;

			using Unpickler unpickler = new();
			using MemoryStream memoryStream = new(playerStatesData);
			ArrayList players = unpickler.load(memoryStream) as ArrayList ?? new ArrayList();

			foreach (ArrayList player in players)
				AddPlayerToReplay(player);
		}

		private void AddPlayerToReplay(ArrayList properties)
		{
			IVersionMapping mapping = _versionMappingFactory.GetMappings(Replay!.ClientVersion);
			ReplayPlayer replayPlayer = new(mapping.ShipConfigMapping);
			foreach (object[] propertyArray in properties)
			{
				string? propertyName = mapping.ReplayPlayerPropertyMappings.GetValueOrDefault((int)propertyArray[0]);
				if (string.IsNullOrEmpty(propertyName))
					continue;

				PropertyInfo? propertyInfo = ReplayPlayer.PropertyInfos.SingleOrDefault(x => x.Name == propertyName);
				if (propertyInfo is null)
					continue;

				propertyInfo.SetValue(replayPlayer, Convert.ChangeType(propertyArray[1], propertyInfo.PropertyType), null);
			}
			ExtendedReplay.ReplayPlayers.Add(replayPlayer);
		}
	}
}
