namespace Nodsoft.WowsReplaysUnpack.Data;

public sealed record ReplayMessage(uint EntityId, string MessageGroup, string MessageContent);
