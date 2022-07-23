namespace Nodsoft.WowsReplaysUnpack.Core.Exceptions;

/// <summary>
/// Exception thrown when a replay is not valid.
/// </summary>
public sealed class InvalidReplayException : ApplicationException
{
	public InvalidReplayException(string? message = null) : base(message) { }
}