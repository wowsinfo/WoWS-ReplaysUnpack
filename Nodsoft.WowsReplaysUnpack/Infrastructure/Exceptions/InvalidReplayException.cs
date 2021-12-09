using System;

namespace Nodsoft.WowsReplaysUnpack.Infrastructure.Exceptions;

public class InvalidReplayException : Exception
{
	public InvalidReplayException(string? message = null) : base(message)
	{
	}
}