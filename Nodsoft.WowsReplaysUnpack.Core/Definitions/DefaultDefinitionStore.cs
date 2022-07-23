using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.DataTypes;
using Nodsoft.WowsReplaysUnpack.Core.Exceptions;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Reflection;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

/// <summary>
/// Default implementation of a Definition store, which is used to load definitions from XML files.
/// </summary>
public class DefaultDefinitionStore : IDefinitionStore
{
	/// <summary>
	/// File name of the entities definition file.
	/// </summary>
	protected const string EntitiesXml = "entities.xml";

	private readonly Version[] _supportedVersions;

	/// <summary>
	/// Logger instance used by this definition store.
	/// </summary>
	protected readonly ILogger<IDefinitionStore> Logger;

	/// <summary>
	/// Assembly of the Definition store (defaults to the implementation assembly).
	/// </summary>
	protected readonly Assembly Assembly;
	
	
	/// <summary>
	/// Version -> Definitions Directory
	/// </summary>
	protected readonly Dictionary<string, DefinitionDirectory> DirectoryCache = new();

	/// <summary>
	/// version_name -> Definition
	/// </summary>
	protected readonly Dictionary<string, EntityDefinition> EntityDefinitionCache = new();

	/// <summary>
	/// version_index -> name
	/// </summary>
	protected readonly Dictionary<string, string> EntityDefinitionIndexNameCache = new();

	/// <summary>
	/// version_name -> index
	/// </summary>
	protected readonly Dictionary<string, int> EntityDefinitionNameIndexCache = new();

	/// <summary>
	/// Version -> DataType Name -> Data Type Name
	/// </summary>
	protected readonly Dictionary<string, Dictionary<string, XmlNode>> TypeMappings = new();

	public DefaultDefinitionStore(ILogger<DefaultDefinitionStore> logger)
	{
		Assembly = typeof(DefaultDefinitionStore).Assembly;
		Logger = logger;

		string versionsDirectory = JoinPath(Assembly.FullName!.GetStringBeforeIndex(Consts.Comma), "Definitions", "Versions");

		_supportedVersions = Assembly.GetManifestResourceNames()
			.Where(name => name.StartsWith(versionsDirectory))
			.Select(name => name.GetStringAfterLength(versionsDirectory + Consts.Dot).GetStringBeforeIndex(Consts.Dot)[1..])
			.Distinct()
			.Select(version => version.Split(Consts.Underscore).Select(int.Parse).ToArray())
			.Select(arr => new Version(arr[0], arr[1], arr[2]))
			.OrderByDescending(version => version)
			.ToArray();
	}

	#region EntityDefinitions

	/// <inheritdoc />
	public virtual EntityDefinition GetEntityDefinition(Version clientVersion, int index)
	{
		clientVersion = GetActualVersion(clientVersion);
		string name = GetEntityDefinitionName(clientVersion, index);

		return GetEntityDefinition(clientVersion, name);
	}

	/// <inheritdoc />
	public virtual EntityDefinition GetEntityDefinition(Version clientVersion, string name)
	{
		clientVersion = GetActualVersion(clientVersion);
		string cacheKey = CacheKey(clientVersion.ToString(), name);

		if (EntityDefinitionCache.TryGetValue(cacheKey, out EntityDefinition? definition))
		{
			return definition;
		}

		definition = new(clientVersion, this, name);
		EntityDefinitionCache.Add(cacheKey, definition);

		return definition;
	}

	/// <summary>
	///	Gets the name of an entity definition by its index.
	/// </summary>
	/// <inheritdoc cref="GetEntityDefinition(System.Version,int)" />
	/// <returns>The name of the entity definition.</returns>
	protected virtual string GetEntityDefinitionName(Version clientVersion, int index)
	{
		string cacheKey = CacheKey(clientVersion.ToString(), (index - 1).ToString());

		if (EntityDefinitionIndexNameCache.TryGetValue(cacheKey, out string? name))
		{
			return name;
		}

		Dictionary<int, string> names = GetEntityIndexes(clientVersion);
		EntityDefinitionIndexNameCache[cacheKey] = names[index];

		return EntityDefinitionIndexNameCache[cacheKey];
	}

	/// <summary>
	/// Gets all entity indexes for a given game client version.
	/// </summary>
	/// <param name="clientVersion">The game client version.</param>
	/// <returns>A dictionary of entity indexes and their names.</returns>
	protected virtual Dictionary<int, string> GetEntityIndexes(Version clientVersion)
	{
		return GetFileAsXml(clientVersion, EntitiesXml).DocumentElement!.SelectSingleNode("ClientServerEntities")!.ChildNodes()
			.Select((node, index) => new { node, index })
			.ToDictionary(i => i.index, i => i.node.Name);
	}

	#endregion

	/// <inheritdoc />
	public virtual XmlDocument GetFileAsXml(Version clientVersion, string name, params string[] directoryNames)
	{
		clientVersion = GetActualVersion(clientVersion);
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

	/// <inheritdoc />
	public virtual DataTypeBase GetDataType(Version clientVersion, XmlNode typeOrArgXmlNode)
	{
		clientVersion = GetActualVersion(clientVersion);
		string versionString = clientVersion.ToString();

		if (TypeMappings.TryGetValue(versionString, out Dictionary<string, XmlNode>? typeMapping))
		{
			return GetDataTypeInternal(clientVersion, typeMapping, typeOrArgXmlNode);
		}

		XmlDocument aliasXml = GetFileAsXml(clientVersion, "alias.xml", "entity_defs");
		typeMapping = new();

		foreach (XmlNode node in aliasXml.DocumentElement!.ChildNodes)
		{
			typeMapping[node.Name.Trim()] = node;
		}

		TypeMappings.Add(versionString, typeMapping);

		return GetDataTypeInternal(clientVersion, typeMapping, typeOrArgXmlNode);
	}

	/// <summary>
	/// Finds a definition directory by given names.
	/// </summary>
	/// <param name="clientVersion">The game client version.</param>
	/// <param name="directoryNames">The names of the directories.</param>
	/// <returns>The definition directory.</returns>
	protected virtual DefinitionDirectory FindDirectory(Version clientVersion, IEnumerable<string> directoryNames)
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
	protected virtual DefinitionDirectory GetRootDirectory(Version clientVersion)
	{
		if (DirectoryCache.TryGetValue(clientVersion.ToString(), out DefinitionDirectory? rootDirectory))
		{
			return rootDirectory;
		}

		string scriptsDirectory = JoinPath(Assembly.FullName!.GetStringBeforeIndex(Consts.Comma),
			"Definitions", "Versions", $"{Consts.Underscore}{clientVersion.ToString().Replace(Consts.Dot, Consts.Underscore)}", "scripts"
		);
		
		string[] fileNames = Assembly.GetManifestResourceNames()
			.Where(name => name.StartsWith(scriptsDirectory))
			.ToArray();

		rootDirectory = new("scripts", scriptsDirectory, fileNames);
		DirectoryCache.Add(clientVersion.ToString(), rootDirectory);

		return rootDirectory;
	}

	/// <summary>
	/// Internal method for getting a data type.
	/// </summary>
	/// <param name="clientVersion">The game client version.</param>
	/// <param name="typeMapping">The type mapping.</param>
	/// <param name="typeOrArgXmlNode">The type or argument XML node.</param>
	/// <returns>The data type.</returns>
	/// <exception cref="NotSupportedException">The data type is not supported.</exception>
	protected virtual DataTypeBase GetDataTypeInternal(Version clientVersion, Dictionary<string, XmlNode> typeMapping, XmlNode typeOrArgXmlNode)
	{
		while (true)
		{
			string typeName = typeOrArgXmlNode.ChildNodes().First(n => n.NodeType is XmlNodeType.Text).TrimmedText();

			if (typeMapping.TryGetValue(typeName, out XmlNode? mappedNode))
			{
				typeOrArgXmlNode = mappedNode;
			}
			else if (TypeConsts.SimpleTypeMappings.TryGetValue(typeName, out Type? dataType))
			{
				return (DataTypeBase)Activator.CreateInstance(dataType, clientVersion, this, typeOrArgXmlNode)!;
			}
			else
			{
				throw new NotSupportedException($"DataType {typeName} is not supported");
			}
		}
	}

	private Version GetActualVersion(Version version)
	{
		Version actualVersion = _supportedVersions.FirstOrDefault(v => version >= v) ?? throw new VersionNotSupportedException(_supportedVersions.Last(), version);

		if (actualVersion != version)
		{
			Logger.LogWarning("The requested version does not match the latest supported version. Requested: {requested}, Latest: {latest}", version, actualVersion);
		}

		return actualVersion;
	}

	/// <summary>
	/// Joins parts of an XML path, separated by a dot.
	/// </summary>
	/// <param name="parts">Parts of the path.</param>
	/// <returns>The joined path.</returns>
	protected static string JoinPath(params string[] parts) => string.Join(Consts.Dot, parts);
	
	/// <summary>
	/// Joins parts of a cache key, separated by an underscore.
	/// </summary>
	/// <param name="values">Parts of the cache key.</param>
	/// <returns>The joined cache key.</returns>
	protected static string CacheKey(params string[] values) => string.Join(Consts.Underscore, values);
}