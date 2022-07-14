using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Core.Network;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Nodsoft.WowsReplaysUnpack.Services;

public class DefaultReplayDataParser : IReplayDataParser
{
	private readonly ILogger<DefaultReplayDataParser> _logger;
	private readonly MemoryStream _packetBuffer = new();
	private readonly BinaryReader _packetBufferReader;

	public DefaultReplayDataParser(ILogger<DefaultReplayDataParser> logger)
	{
		_logger = logger;
		_packetBufferReader = new BinaryReader(_packetBuffer);
	}
	protected static readonly Regex RceInjectionRegex = new(
		@"cnt\ssystem|commands",
		RegexOptions.Compiled | RegexOptions.ExplicitCapture);

	/// <summary>
	/// Parses the individual network packets
	/// </summary>
	/// <param name="replayDataStream"></param>
	public virtual IEnumerable<INetworkPacket> ParseNetworkPackets(MemoryStream replayDataStream, ReplayUnpackerOptions options)
	{
		using BinaryReader binaryReader = new(replayDataStream);
		while (replayDataStream.Position != replayDataStream.Length)
		{
			var packetSize = binaryReader.ReadUInt32();
			var packetType = binaryReader.ReadUInt32();
			var packetTime = binaryReader.ReadSingle();

			_logger.LogDebug("Packet parsed of type '{packetType}' with size '{packetSize}' and timestamp '{packetTime}'",
				NetworkPacketTypes.GetName(packetType), packetSize, packetTime);

			byte[] packetData = binaryReader.ReadBytes((int)packetSize);

			// Reset packet buffer, write current data and set position to start
			_packetBuffer.SetLength(packetSize);
			_packetBuffer.Seek(0, SeekOrigin.Begin);
			_packetBuffer.Write(packetData);
			_packetBuffer.Seek(0, SeekOrigin.Begin);

			yield return packetType switch
			{
				NetworkPacketTypes.BasePlayerCreate => new BasePlayerCreatePacket(_packetBufferReader),
				NetworkPacketTypes.CellPlayerCreate => new CellPlayerCreatePacket(_packetBufferReader),
				NetworkPacketTypes.EntityControl => new EntityControlPacket(_packetBufferReader),
				NetworkPacketTypes.EntityEnter => new EntityEnterPacket(_packetBufferReader),
				NetworkPacketTypes.EntityLeave => new EntityLeavePacket(_packetBufferReader),
				NetworkPacketTypes.EntityCreate => new EntityCreatePacket(_packetBufferReader),
				NetworkPacketTypes.EntityProperty => new EntityPropertyPacket(_packetBufferReader),
				NetworkPacketTypes.EntityMethod => new EntityMethodPacket(_packetBufferReader),
				NetworkPacketTypes.Map => new MapPacket(_packetBufferReader),
				NetworkPacketTypes.NestedProperty => new NestedPropertyPacket(_packetBufferReader),
				NetworkPacketTypes.Position => new PositionPacket(_packetBufferReader),
				NetworkPacketTypes.PlayerPosition => new PlayerPositionPacket(_packetBufferReader),
				_ => new UnknownPacket(_packetBufferReader)
			};
		}
	}

	public void Dispose()
	{
		_packetBufferReader.Dispose();
		GC.SuppressFinalize(this);
	}
}
