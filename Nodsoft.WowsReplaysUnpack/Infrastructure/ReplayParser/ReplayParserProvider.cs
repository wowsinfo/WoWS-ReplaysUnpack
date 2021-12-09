﻿using Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser.Versions;
using System;


namespace Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser;


public class ReplayParserProvider
{
	/// <summary>
	/// Provides a replay parser tailored to a Replay's game version.
	/// </summary>
	/// <param name="version">Game Version of the Replay file.</param>
	/// <returns><see cref="IReplayParser"/> for specified version.</returns>
	/// <exception cref="NotSupportedException">Specified version is not supported (yet), or is out of range.</exception>
	public static IReplayParser FromReplayVersion(Version version)
	{
		// Match versions, newest to oldest.
		if (version >= new Version(0, 10, 11)) return new ReplayParser_0_10_11();
		if (version == new Version(0, 10, 10)) return new ReplayParser_0_10_10();


		// No version was matched.
		throw new NotSupportedException($"No supported parser was found for Version {version}.");
	}
}