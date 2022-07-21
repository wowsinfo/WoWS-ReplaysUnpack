namespace Nodsoft.WowsReplaysUnpack.ExtendedData.Models;

public enum ReplayMessageGroup
{
	Unknown = 0,
	Team = 1,
	All = 2
}
public record ChatMessage
{
	public uint EntityId { get; }
	public float PacketTime { get; } // Time in seconds from battle start
	public TimeSpan SentTime => TimeSpan.FromSeconds(PacketTime);
	public ReplayMessageGroup MessageGroup { get; }
	public string MessageContent { get; }

	public ChatMessage(uint entityId, float packetTime, string messageGroup, string messageContent)
	{
		EntityId = entityId;
		PacketTime = packetTime;
		MessageGroup = ParseMessageGroup(messageGroup);
		MessageContent = messageContent;
	}

	private static ReplayMessageGroup ParseMessageGroup(string messageGroup) => messageGroup switch
	{
		"battle_team" => ReplayMessageGroup.Team,
		"battle_common" => ReplayMessageGroup.All,
		_ => ReplayMessageGroup.Unknown,
	};
}
