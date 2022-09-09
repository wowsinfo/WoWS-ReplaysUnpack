using Nodsoft.WowsReplaysUnpack.Core.DataTypes;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

/// <summary>
/// Specifies a definition store, responsible for managing, accessing and caching
/// the .def files (used for type and property mapping).
/// </summary>
public interface IDefinitionStore
{
	/// <summary>
	/// The Definition loader used to load this store's definitions.
	/// </summary>
	IDefinitionLoader Loader { get; }

	/// <summary>
	/// Gets the data type of an XML node.
	/// </summary>
	/// <param name="clientVersion">Game client version</param>
	/// <param name="typeOrArgXmlNode">XML node to get the data type of</param>
	/// <returns>Data type of the XML node</returns>
	DataTypeBase GetDataType(Version clientVersion, XmlNode typeOrArgXmlNode);
	
	/// <summary>
	/// Gets an entity definition by its index.
	/// </summary>
	/// <param name="clientVersion">Game client version</param>
	/// <param name="index">Index of the entity definition</param>
	/// <returns>Entity definition</returns>
	EntityDefinition GetEntityDefinition(Version clientVersion, int index);
	
	/// <summary>
	/// Gets a property definition by its name.
	/// </summary>
	/// <param name="clientVersion">Game client version</param>
	/// <param name="name">Name of the property definition</param>
	/// <returns>Property definition</returns>
	EntityDefinition GetEntityDefinition(Version clientVersion, string name);
}
