namespace Nodsoft.WowsReplaysUnpack.Core.Exceptions;

public class InvalidReplayException : Exception
{
	public InvalidReplayException(string? message = null) : base(message) { }
}