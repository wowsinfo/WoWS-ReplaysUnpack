using Nodsoft.WowsReplaysUnpack.Core.DataTypes;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Reflection;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;
public interface IDefinitionStore
{
	ADataTypeBase GetDataType(Version clientVersion, XmlNode typeOrArgXmlNode);
	EntityDefinition GetEntityDefinitionByIndex(Version clientVersion, int index);
	EntityDefinition GetEntityDefinitionByName(Version clientVersion, string name);
	XmlDocument GetFileAsXml(Version clientVersion, string name, params string[] directoryNames);
}
public class DefaultDefinitionStore : IDefinitionStore
{
	protected const string ENTITIES_XML = "entities.xml";
	protected readonly Assembly _assembly;

	/// <summary>
	/// Version -> Definitions Directory
	/// </summary>
	protected readonly Dictionary<string, DefinitionDirectory> _directoryCache = new();

	/// <summary>
	/// version_name -> Definition
	/// </summary>
	protected readonly Dictionary<string, EntityDefinition> _EntityDefinitionCache = new();

	/// <summary>
	/// version_index -> name
	/// </summary>
	protected readonly Dictionary<string, string> _entityDefinitionIndexNameCache = new();
	/// <summary>
	/// version_name -> index
	/// </summary>
	protected readonly Dictionary<string, int> _entityDefinitionNameIndexCache = new();

	/// <summary>
	/// Version -> DataType Name -> Data Type Name
	/// </summary>
	protected readonly Dictionary<string, Dictionary<string, XmlNode>> _typeMappings = new();

	public DefaultDefinitionStore()
	{
		_assembly = typeof(DefaultDefinitionStore).Assembly;
	}

	#region EntityDefinitions
	public virtual EntityDefinition GetEntityDefinitionByIndex(Version clientVersion, int index)
	{
		var name = GetEntityDefinitionNameByIndex(clientVersion, index);
		return GetEntityDefinitionByName(clientVersion, name);
	}
	public virtual EntityDefinition GetEntityDefinitionByName(Version clientVersion, string name)
	{
		var cacheKey = CacheKey(clientVersion.ToString(), name);
		if (_EntityDefinitionCache.TryGetValue(cacheKey, out var definition))
			return definition;

		definition = new EntityDefinition(clientVersion, this, name);
		_EntityDefinitionCache.Add(cacheKey, definition);
		return definition;
	}

	//private int GetEntityDefinitionIndexByName(Version clientVersion, string name)
	//{
	//	var cacheKey = CacheKey(clientVersion.ToString(), name);
	//	if (_entityDefinitionNameIndexCache.TryGetValue(cacheKey, out var index))
	//		return index;

	//	var indexes = GetEntityIndexes(clientVersion);
	//	_entityDefinitionNameIndexCache[cacheKey] = indexes.Single(kv => kv.Value == name).Key;
	//	return _entityDefinitionNameIndexCache[cacheKey];
	//}

	protected virtual string GetEntityDefinitionNameByIndex(Version clientVersion, int index)
	{
		var cacheKey = CacheKey(clientVersion.ToString(), index.ToString());
		if (_entityDefinitionIndexNameCache.TryGetValue(cacheKey, out var name))
			return name;

		var names = GetEntityIndexes(clientVersion);
		_entityDefinitionIndexNameCache[cacheKey] = names[index];
		return _entityDefinitionIndexNameCache[cacheKey];
	}

	protected virtual Dictionary<int, string> GetEntityIndexes(Version clientVersion)
	{
		return GetFileAsXml(clientVersion, ENTITIES_XML).DocumentElement!.SelectSingleNode("ClientServerEntities")!.ChildNodes()
			.Select((node, index) => new { node, index }).ToDictionary(i => i.index, i => i.node.Name);
	}
	#endregion


	public virtual XmlDocument GetFileAsXml(Version clientVersion, string name, params string[] directoryNames)
	{
		var directory = FindDirectory(clientVersion, directoryNames);
		var file = directory.Files.SingleOrDefault(f => f.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
		if (file is null)
			throw new Exception("File could not be found");
		var settings = new XmlReaderSettings
		{
			IgnoreComments = true
		};
		var reader = XmlReader.Create(_assembly.GetManifestResourceStream(file.Path) ?? throw new Exception("File not found"), settings);
		var xmlDocument = new XmlDocument();
		xmlDocument.Load(reader);
		return xmlDocument;
	}


	protected virtual DefinitionDirectory FindDirectory(Version clientVersion, string[] directoryNames)
	{
		DefinitionDirectory folder = GetRootDirectory(clientVersion);
		foreach (var folderName in directoryNames)
		{
			var foundFolder = folder.Directories.SingleOrDefault(f => f.Name.Equals(folderName, StringComparison.InvariantCultureIgnoreCase));
			if (foundFolder is null)
				break;
			folder = foundFolder;
		}
		return folder;
	}

	protected virtual DefinitionDirectory GetRootDirectory(Version clientVersion)
	{
		if (_directoryCache.TryGetValue(clientVersion.ToString(), out var rootDirectory))
			return rootDirectory;

		var scriptsDirectory = JoinPath(_assembly.FullName!.GetStringBeforeIndex(','), "Definitions", "Versions", "_" + clientVersion.ToString().Replace('.', '_'), "scripts");
		var fileNames = _assembly.GetManifestResourceNames()
			.Where(name => name.StartsWith(scriptsDirectory))
			.ToArray();

		rootDirectory = new DefinitionDirectory("scripts", scriptsDirectory, fileNames);
		_directoryCache.Add(clientVersion.ToString(), rootDirectory);
		return rootDirectory;
	}

	public virtual ADataTypeBase GetDataType(Version clientVersion, XmlNode typeOrArgXmlNode)
	{
		var versionString = clientVersion.ToString();
		if (_typeMappings.ContainsKey(versionString) && _typeMappings.TryGetValue(versionString, out var typeMapping))
			return GetDataTypeInternal(clientVersion, typeMapping, typeOrArgXmlNode);

		var aliasXml = GetFileAsXml(clientVersion, "alias.xml", "entity_defs");
		typeMapping = new Dictionary<string, XmlNode>();
		foreach (XmlNode node in aliasXml.DocumentElement!.ChildNodes)
		{
			typeMapping[node.Name.Trim()] = node;
		}
		_typeMappings.Add(versionString, typeMapping);
		return GetDataTypeInternal(clientVersion, typeMapping, typeOrArgXmlNode);
	}
	protected virtual ADataTypeBase GetDataTypeInternal(Version clientVersion, Dictionary<string, XmlNode> typeMapping, XmlNode typeOrArgXmlNode)
	{
		var typeName = typeOrArgXmlNode.ChildNodes().First(n => n.NodeType is XmlNodeType.Text).TrimmedText();
		if (typeMapping.TryGetValue(typeName, out var mappedNode))
			return GetDataTypeInternal(clientVersion, typeMapping, mappedNode);
		else if (TypeConsts.SimpleTypeMappings.TryGetValue(typeName, out var dataType))
			return (ADataTypeBase)Activator.CreateInstance(dataType, clientVersion, this, typeOrArgXmlNode)!;
		else
			throw new NotSupportedException($"DataType {typeName} is not supported");
	}

	protected static string JoinPath(params string[] parts)
		=> string.Join(".", parts);
	protected static string CacheKey(params string[] values)
		=> string.Join('_', values);
}
