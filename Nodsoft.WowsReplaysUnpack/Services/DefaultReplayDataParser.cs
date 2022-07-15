using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Core.Network;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;
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
	public virtual IEnumerable<ANetworkPacket> ParseNetworkPackets(MemoryStream replayDataStream, ReplayUnpackerOptions options)
	{
		int packetIndex = 0;
		using BinaryReader binaryReader = new(replayDataStream);
		while (replayDataStream.Position != replayDataStream.Length)
		{
			uint packetSize = binaryReader.ReadUInt32();
			uint packetType = binaryReader.ReadUInt32();
			float packetTime = binaryReader.ReadSingle(); // Time in seconds from battle start

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
				NetworkPacketTypes.BasePlayerCreate => new BasePlayerCreatePacket(packetIndex, _packetBufferReader),
				NetworkPacketTypes.CellPlayerCreate => new CellPlayerCreatePacket(packetIndex, _packetBufferReader),
				NetworkPacketTypes.EntityControl => new EntityControlPacket(packetIndex, _packetBufferReader),
				NetworkPacketTypes.EntityEnter => new EntityEnterPacket(packetIndex, _packetBufferReader),
				NetworkPacketTypes.EntityLeave => new EntityLeavePacket(packetIndex, _packetBufferReader),
				NetworkPacketTypes.EntityCreate => new EntityCreatePacket(packetIndex, _packetBufferReader),
				NetworkPacketTypes.EntityProperty => new EntityPropertyPacket(packetIndex, _packetBufferReader),
				NetworkPacketTypes.EntityMethod => new EntityMethodPacket(packetIndex, packetTime, _packetBufferReader),
				NetworkPacketTypes.Map => new MapPacket(packetIndex, _packetBufferReader),
				NetworkPacketTypes.NestedProperty => new NestedPropertyPacket(packetIndex, _packetBufferReader),
				NetworkPacketTypes.Position => new PositionPacket(packetIndex, _packetBufferReader),
				NetworkPacketTypes.PlayerPosition => new PlayerPositionPacket(packetIndex, _packetBufferReader),
				_ => new UnknownPacket(packetIndex, _packetBufferReader)
			};
			packetIndex++;
		}
	}

	public void Dispose()
	{
		_packetBufferReader.Dispose();
		GC.SuppressFinalize(this);
	}
}
