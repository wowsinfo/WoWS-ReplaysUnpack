using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Exceptions;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

/// <summary>
/// Provides a definition loader to load entity definitions from XML files embedded in the library's assembly.
/// </summary>
public class EmbeddedDefinitionLoader : IDefinitionLoader
{
	/// <summary>
	/// Version -> Definitions Directory
	/// </summary>
	protected readonly Dictionary<string, DefinitionDirectory> DirectoryCache = new();

	/// <summary>
	/// Logger instance used by this definition store.
	/// </summary>
	protected readonly ILogger<IDefinitionLoader> Logger;
	
	protected internal Assembly Assembly { get; }

	/// <summary>
	/// Lists all the game client versions that are supported by this definition loader.
	/// </summary>
	public IEnumerable<Version> SupportedVersions => _supportedVersions;

	private readonly Version[] _supportedVersions;

	public EmbeddedDefinitionLoader(ILogger<EmbeddedDefinitionLoader> logger)
	{
		Logger = logger;
		Assembly = typeof(EmbeddedDefinitionLoader).Assembly;
		
		string versionsDirectory = string.Join('.', Assembly.FullName!.GetStringBeforeIndex(','), nameof(Definitions), "Versions");
		
		_supportedVersions = Assembly.GetManifestResourceNames()
			.Where(name => name.StartsWith(versionsDirectory))
			.Select(name => name.GetStringAfterLength(versionsDirectory + '.').GetStringBeforeIndex('.')[1..])
			.Distinct()
			.Select(static version => version.Split('_').Select(int.Parse).ToArray())
			.Select(static arr => new Version(arr[0], arr[1], arr[2]))
			.OrderByDescending(static version => version)
			.ToArray();
	}

	/// <inheritdoc />
	public virtual XmlDocument GetFileAsXml(Version clientVersion, string name, params string[] directoryNames)
	{
		clientVersion = GetExactVersion(clientVersion);
		DefinitionDirectory directory = FindDirectory(clientVersion, directoryNames);

		if (directory.Files.SingleOrDefault(f => f.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) is not { } file)
		{
			throw new InvalidOperationException("File could not be found");
		}

		XmlReaderSettings settings = new() { IgnoreComments = true };
		XmlReader reader = XmlReader.Create(Assembly.GetManifestResourceStream(file.Path) ?? throw new InvalidOperationException("File not found"), settings);
		XmlDocument xmlDocument = new();
		xmlDocument.Load(reader);

		return xmlDocument;
	}

	/// <summary>
	/// Finds a definition directory by given names.
	/// </summary>
	/// <param name="clientVersion">The game client version.</param>
	/// <param name="directoryNames">The names of the directories.</param>
	/// <returns>The definition directory.</returns>
	public virtual DefinitionDirectory FindDirectory(Version clientVersion, IEnumerable<string> directoryNames)
	{
		DefinitionDirectory folder = GetRootDirectory(clientVersion);

		foreach (string? folderName in directoryNames)
		{
			DefinitionDirectory? foundFolder = folder.Directories.SingleOrDefault(f => f.Name.Equals(folderName, StringComparison.InvariantCultureIgnoreCase));

			if (foundFolder is null)
			{
				break;
			}

			folder = foundFolder;
		}

		return folder;
	}

	/// <summary>
	/// Gets the root definition directory for a given game client version.
	/// </summary>
	/// <param name="clientVersion">The game client version.</param>
	/// <returns>The root definition directory.</returns>
	public virtual DefinitionDirectory GetRootDirectory(Version clientVersion)
	{
		if (DirectoryCache.TryGetValue(clientVersion.ToString(), out DefinitionDirectory? rootDirectory))
		{
			return rootDirectory;
		}

		string scriptsDirectory = string.Join('.', Assembly.FullName!.GetStringBeforeIndex(','),
			"Definitions", "Versions", $"_{clientVersion.ToString().Replace('.', '_')}", "scripts"
		);

		string[] fileNames = Assembly.GetManifestResourceNames()
			.Where(name => name.StartsWith(scriptsDirectory))
			.ToArray();

		rootDirectory = new("scripts", scriptsDirectory, fileNames);
		DirectoryCache.Add(clientVersion.ToString(), rootDirectory);

		return rootDirectory;
	}
	
	/// <summary>
	/// Gets an exact version supported by this definition loader, or the closest version below it.
	/// </summary>
	/// <param name="version">The version to find.</param>
	/// <returns>The closest version supported by this definition loader.</returns>
	/// <exception cref="VersionNotSupportedException">Thrown when no version is supported by this definition loader.</exception>
	public Version GetExactVersion(Version version)
	{
		Version actualVersion = SupportedVersions.FirstOrDefault(v => version >= v) ?? throw new VersionNotSupportedException(_supportedVersions.Last(), version);

		if (actualVersion != version)
		{
			Logger.LogWarning("The requested version does not match the latest supported version. Requested: {requested}, Latest: {latest}", version, actualVersion);
		}

		return actualVersion;
	}
}