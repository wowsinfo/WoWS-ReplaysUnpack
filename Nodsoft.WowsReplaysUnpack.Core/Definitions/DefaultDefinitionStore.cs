using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.DataTypes;
using Nodsoft.WowsReplaysUnpack.Core.Exceptions;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Reflection;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

public class DefaultDefinitionStore : IDefinitionStore
{
	protected const string EntitiesXml = "entities.xml";

	private readonly Version[] _supportedVersions;

	protected readonly ILogger<IDefinitionStore> Logger;

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

	public virtual EntityDefinition GetEntityDefinition(Version clientVersion, int index)
	{
		clientVersion = GetActualVersion(clientVersion);
		string name = GetEntityDefinitionNameByIndex(clientVersion, index);

		return GetEntityDefinition(clientVersion, name);
	}

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

	protected virtual string GetEntityDefinitionNameByIndex(Version clientVersion, int index)
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

	protected virtual Dictionary<int, string> GetEntityIndexes(Version clientVersion)
	{
		return GetFileAsXml(clientVersion, EntitiesXml).DocumentElement!.SelectSingleNode("ClientServerEntities")!.ChildNodes()
			.Select((node, index) => new { node, index })
			.ToDictionary(i => i.index, i => i.node.Name);
	}

	#endregion


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

	protected virtual DataTypeBase GetDataTypeInternal(Version clientVersion, Dictionary<string, XmlNode> typeMapping, XmlNode typeOrArgXmlNode)
	{
		while (true)
		{
			string typeName = typeOrArgXmlNode.ChildNodes().First(n => n.NodeType is XmlNodeType.Text).TrimmedText();

			if (typeMapping.TryGetValue(typeName, out XmlNode? mappedNode))
			{
				typeOrArgXmlNode = mappedNode;

				continue;
			}

			if (TypeConsts.SimpleTypeMappings.TryGetValue(typeName, out Type? dataType))
			{
				return (DataTypeBase)Activator.CreateInstance(dataType, clientVersion, this, typeOrArgXmlNode)!;
			}

			throw new NotSupportedException($"DataType {typeName} is not supported");
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

	protected static string JoinPath(params string[] parts) => string.Join(Consts.Dot, parts);
	protected static string CacheKey(params string[] values) => string.Join(Consts.Underscore, values);
}