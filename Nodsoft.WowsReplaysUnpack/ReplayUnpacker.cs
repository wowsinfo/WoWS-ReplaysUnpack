using Nodsoft.WowsReplaysUnpack.Data;
using Nodsoft.WowsReplaysUnpack.Infrastructure;
using Razorvine.Pickle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Nodsoft.WowsReplaysUnpack;


public class ReplayUnpacker
{
	public ReplayRaw UnpackReplay(Stream stream)
	{


		byte[] bReplaySignature = new byte[4];
		byte[] bReplayBlockCount = new byte[4];
		byte[] bReplayBlockSize = new byte[4];
		byte[] bReplayJSONData;

		stream.Read(bReplaySignature, 0, 4);
		stream.Read(bReplayBlockCount, 0, 4);
		stream.Read(bReplayBlockSize, 0, 4);

		int jsonDataSize = BitConverter.ToInt32(bReplayBlockSize, 0);

		bReplayJSONData = new byte[jsonDataSize];
		stream.Read(bReplayJSONData, 0, jsonDataSize);

		ReplayRaw replay = new() { ArenaInfoJson = Encoding.UTF8.GetString(bReplayJSONData) };

		using MemoryStream memStream = new();
		stream.CopyTo(memStream);

		string sBfishKey = "\x29\xB7\xC9\x09\x38\x3F\x84\x88\xFA\x98\xEC\x4E\x13\x19\x79\xFB";
		byte[] bBfishKey = sBfishKey.Select(x => Convert.ToByte(x)).ToArray();
		BlowFish bfish = new(bBfishKey);
		long prev = 0;

		using MemoryStream compressedData = new();

		foreach ((int, byte[]) chunk in ChunkData(memStream.ToArray()[8..]))
		{
			try
			{
				long decrypted_block = BitConverter.ToInt64(bfish.Decrypt_ECB(chunk.Item2));

				if (prev is not 0)
				{
					decrypted_block ^= prev;
				}

				prev = decrypted_block;
				compressedData.Write(BitConverter.GetBytes(decrypted_block));
			}
			catch (ArgumentOutOfRangeException)	{ }
		}

		compressedData.Seek(2, SeekOrigin.Begin); //DeflateStream doesn't strip the header so we strip it manually.
		MemoryStream decompressedData = new();

		using (DeflateStream df = new(compressedData, CompressionMode.Decompress))
		{
			df.CopyTo(decompressedData);
		}

		decompressedData.Seek(0, SeekOrigin.Begin);

		while (decompressedData.Position != decompressedData.Length)
		{
			NetPacket np = new(decompressedData);

			if (np.Type is "08")
			{
				EntityMethod em = new(np.RawData);

				if (em.MessageId is 124) // 10.10=124, OnArenaStatesReceived
				{
					//var unk1 = new byte[8]; //?
					//em.Data.Value.Read(unk1);

					byte[] arenaID = new byte[8];
					em.Data.Value.Read(arenaID);

					byte[] teamBuildTypeID = new byte[1];
					em.Data.Value.Read(teamBuildTypeID);

					byte[] blobPreBattlesInfoSize = new byte[1];
					em.Data.Value.Read(blobPreBattlesInfoSize);

					byte[] blobPreBattlesInfo = new byte[blobPreBattlesInfoSize[0]];
					em.Data.Value.Read(blobPreBattlesInfo);

					byte[] blobPlayersStatesSize = new byte[1];
					em.Data.Value.Read(blobPlayersStatesSize);

					if (blobPlayersStatesSize[0] is 255)
					{
						byte[] blobPlayerStatesRealSize = new byte[2];
						em.Data.Value.Read(blobPlayerStatesRealSize);
						ushort PlayerStatesRealSize = BitConverter.ToUInt16(blobPlayerStatesRealSize);
						em.Data.Value.Read(new byte[1]); //?

						// blobPlayerStates will contain players' information like account id, server realm, etc...
						// but it is serialized via Python's pickle.
						// We use Razorvine's Pickle Unpickler for that.

						byte[] blobPlayerStates = new byte[PlayerStatesRealSize];
						em.Data.Value.Read(blobPlayerStates);

						Unpickler.registerConstructor(nameof(CamouflageInfo), nameof(CamouflageInfo), new CamouflageInfo());
						Unpickler k = new();

						foreach (ArrayList player in k.load(new MemoryStream(blobPlayerStates)) as ArrayList)
						{
							replay.ReplayPlayers.Add(new() { Properties = player.ToArray() });
						}

						/*
							...
							accountDBID          : 2016494874
							avatarId             : 919187
							camouflageInfo       : 4205768624, 0
							clanColor            : 13427940
							clanID               : 2000008825
							clanTag              : TF44
							crewParams           : System.Collections.ArrayList
							dogTag               : System.Collections.ArrayList
							fragsCount           : 0
							friendlyFireEnabled  : False
							id                   : 537149649
							invitationsEnabled   : True
							isAbuser             : False
							isAlive              : True
							isBot                : False
							isClientLoaded       : False
							isConnected          : True
							isHidden             : False
							isLeaver             : False
							isPreBattleOwner     : False
							killedBuildingsCount : 0
							maxHealth            : 27500
							name                 : notyourfather
							playerMode           : Razorvine.Pickle.Objects.ClassDict
							preBattleIdOnStart   : 537256655
							preBattleSign        : 0
							prebattleId          : 537256655
							realm                : ASIA
							shipComponents       : System.Collections.Hashtable
							shipId               : 919188
							shipParamsId         : 4288591856
							skinId               : 4288591856
							teamId               : 0
							ttkStatus            : False
							...
						 */
					}

				}
				else if (em.MessageId is 122) // 10.10=122, OnChatMessage
				{
					byte[] bEntityId = new byte[4];
					em.Data.Value.Read(bEntityId);
					uint entityId = BitConverter.ToUInt32(bEntityId);

					byte[] bMessageGroupSize = new byte[1];
					em.Data.Value.Read(bMessageGroupSize);
					byte[] bMessageGroup = new byte[bMessageGroupSize[0]];
					em.Data.Value.Read(bMessageGroup);
					string messageGroup = Encoding.UTF8.GetString(bMessageGroup);

					byte[] bMessageContentSize = new byte[1];
					em.Data.Value.Read(bMessageContentSize);
					byte[] bMessageContent = new byte[bMessageContentSize[0]];
					em.Data.Value.Read(bMessageContent);
					string messageContent = Encoding.UTF8.GetString(bMessageContent);

					replay.ChatMessages.Add(new(entityId, messageGroup, messageContent));
					/*
						615476 : battle_team : cv run
						615474 : battle_common : nb
						615488 : battle_team : lol
						615452 : battle_team : lol
						615480 : battle_team : ???????bug?
						615480 : battle_team : ?????????
						615480 : battle_team : ??
						615474 : battle_common : ??????
						615452 : battle_team : ???????
						615480 : battle_team : ???
						615480 : battle_team : ??
						615480 : battle_team : ????`
						615480 : battle_team : ?TM???
						615452 : battle_team : ??
						615480 : battle_team : ????????
						615480 : battle_team : ????????
						615480 : battle_team : ??
						615452 : battle_team : ? ???????
					 */
				}
			}
		}

		return replay;
	}


	internal static IEnumerable<(int, byte[])> ChunkData(byte[] data, int len = 8)
	{
		int idx = 0;

		for (int s = 0; s <= data.Length; s += len)
		{
			byte[] g;

			try
			{
				g = data[s..(s + len)];
			}
			catch (ArgumentOutOfRangeException)
			{
				g = data[s..];
			}

			idx++;
			yield return (idx, g);
		}
	}
}
