using Nodsoft.WowsReplaysUnpack.Core.DataTypes;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Reflection;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

public class DefinitionStore
{
	private readonly Assembly _assembly;

	/// <summary>
	/// Version -> Definitions Directory
	/// </summary>
	private readonly Dictionary<string, DefinitionDirectory> _directoryCache = new();

	/// <summary>
	/// Definition cache key -> Definition
	/// </summary>
	private readonly Dictionary<string, ABaseDefinition> _definitionCache = new();

	/// <summary>
	/// Version -> DataType Name -> Data Type Name
	/// </summary>
	private readonly Dictionary<string, Dictionary<string, XmlNode>> _typeMappings = new();

	public DefinitionStore()
	{
		_assembly = typeof(DefinitionStore).Assembly;
	}

	//public EntityDefinition GetEntityDefinition(Version clientVersion, string name, params string[] directoryNames)
	//{
	//	string cacheKey = GetCacheKey(clientVersion, name, directoryNames);
	//	if (_definitionCache.TryGetValue(cacheKey, out ABaseDefinition definition))
	//		return (EntityDefinition)definition;

	//	definition = new EntityDefinition()
	//}

	private string GetCacheKey(Version clientVersion, string name, params string[] directoryNames)
		  => clientVersion.ToString() + "_" + name + (directoryNames?.Length > 0 ? "_" + string.Join('_', directoryNames) : string.Empty);



	public XmlDocument GetFileAsXml(Version clientVersion, string name, params string[] directoryNames)
	{
		var directory = FindDirectory(clientVersion, directoryNames);
		var file = directory.Files.SingleOrDefault(f => f.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
		if (file is null)
			throw new Exception("File could not be found");

		var xmlDocument = new XmlDocument();
		xmlDocument.Load(_assembly.GetManifestResourceStream(file.Path) ?? throw new Exception("File not found"));
		return xmlDocument;
	}


	private DefinitionDirectory FindDirectory(Version clientVersion, string[] directoryNames)
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

	private DefinitionDirectory GetRootDirectory(Version clientVersion)
	{
		if (_directoryCache.TryGetValue(clientVersion.ToString(), out var rootDirectory))
			return rootDirectory;

		var scriptsDirectory = JoinPath(_assembly.FullName!.GetStringBeforeIndex(','), "Versions", "_" + clientVersion.ToString().Replace('.', '_'), "scripts");
		var fileNames = _assembly.GetManifestResourceNames()
			.Where(name => name.StartsWith(scriptsDirectory))
			.ToArray();

		rootDirectory = new DefinitionDirectory("scripts", scriptsDirectory, fileNames);
		_directoryCache.Add(clientVersion.ToString(), rootDirectory);
		return rootDirectory;
	}

	private static string JoinPath(params string[] parts)
		=> string.Join(".", parts);

	public ADataTypeBase GetDataType(Version clientVersion, XmlNode xmlNode)
	{
		var versionString = clientVersion.ToString();
		if (_typeMappings.ContainsKey(versionString) && _typeMappings.TryGetValue(versionString, out Dictionary<string, XmlNode> typeMapping))
			return GetDataTypeInternal(typeMapping, xmlNode);

		var aliasXml = GetFileAsXml(clientVersion, "alias.xml", "entity_defs");
		typeMapping = new Dictionary<string, XmlNode>();
		foreach (XmlNode node in aliasXml.DocumentElement!.ChildNodes)
		{
			typeMapping[node.Name.Trim()] = node;
		}
		_typeMappings.Add(versionString, typeMapping);
		return GetDataTypeInternal(typeMapping, xmlNode);
	}
	private ADataTypeBase GetDataTypeInternal(Dictionary<string, XmlNode> typeMapping, XmlNode xmlNode)
	{
		var tagName = xmlNode.Name.Trim();
		if (typeMapping.TryGetValue(xmlNode.Name.Trim(), out var mappedNode))
			return GetDataTypeInternal(typeMapping, mappedNode);
		else if (AliasConsts.SimpleTypeMappings.TryGetValue(tagName, out var dataType))
			return (ADataTypeBase)Activator.CreateInstance(dataType, xmlNode)!;
		else
			throw new NotSupportedException($"DataType {tagName} is not supported");
	}
}
