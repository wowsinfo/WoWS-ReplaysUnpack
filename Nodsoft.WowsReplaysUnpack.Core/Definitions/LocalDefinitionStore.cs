using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.DataTypes;
using Nodsoft.WowsReplaysUnpack.Core.Exceptions;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

/// <summary>
/// An extenstion to <see cref="DefaultDefinitionStore"/>, which is used to load definitions from local XML files.
/// </summary>
public class LocalDefinitionStore : DefaultDefinitionStore
{
	private readonly string executableDirectory;

	public LocalDefinitionStore(ILogger<DefaultDefinitionStore> logger): base(logger) {
		executableDirectory = Path.GetDirectoryName(Environment.ProcessPath) ?? throw new DirectoryNotFoundException();
	}

	protected override Version[] GetSupportedVersions()
	{
		 return Directory.GetDirectories(Path.Join(executableDirectory, "Versions"))
			.Select(name =>
			{
				return name[$"Versions\\".Length..];
			})
			.Distinct()
			.Select(version => version.Split('_').Select(int.Parse).ToArray())
			.Select(arr => new Version(arr[0], arr[1], arr[2]))
			.OrderByDescending(version => version)
			.ToArray();
	}

	/// <summary>
	/// Gets the root definition directory for a given game client version.
	/// </summary>
	/// <param name="clientVersion">The game client version.</param>
	/// <returns>The root definition directory.</returns>
	protected override DefinitionDirectory GetRootDirectory(Version clientVersion)
	{
		if (DirectoryCache.TryGetValue(clientVersion.ToString(), out DefinitionDirectory? rootDirectory))
		{
			return rootDirectory;
		}

		string scriptsDirectory = Path.Join(executableDirectory,
			"Versions", $"{clientVersion.ToString().Replace('.', '_')}", "scripts"
		);

		string[] fileNames = Directory.GetFiles(scriptsDirectory, "*", SearchOption.AllDirectories);

		rootDirectory = new("scripts", scriptsDirectory, fileNames);
		DirectoryCache.Add(clientVersion.ToString(), rootDirectory);

		return rootDirectory;
	}

	public override XmlDocument GetFileAsXml(Version clientVersion, string name, params string[] directoryNames)
	{
		clientVersion = GetActualVersion(clientVersion);
		DefinitionDirectory directory = FindDirectory(clientVersion, directoryNames);

		// Additional strings presents in Name, only matching the end is required
		if (directory.Files.SingleOrDefault(f => f.Name.EndsWith(name, StringComparison.InvariantCultureIgnoreCase)) is not { } file)
		{
			throw new InvalidOperationException("File could not be found");
		}

		XmlReaderSettings settings = new() { IgnoreComments = true };
		// Read from the local path instead
		XmlReader reader = XmlReader.Create(file.Path, settings);
		XmlDocument xmlDocument = new();
		xmlDocument.Load(reader);

		return xmlDocument;
	}
}