using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

public class EntityDefinition : ABaseDefinition
{
	private const string ENTITY_DEFS = "entity_defs";

	public List<EntityMethodDefinition> CellMethods { get; set; } = new();
	public List<EntityMethodDefinition> BaseMethods { get; set; } = new();
	public List<EntityMethodDefinition> ClientMethods { get; set; } = new();

	public EntityDefinition(Version clientVersion, IDefinitionStore definitionStore,
		string name) : base(clientVersion, definitionStore, name, ENTITY_DEFS)
	{
	}

	protected override void ParseDefinitionFile(XmlElement xml)
	{
		base.ParseDefinitionFile(xml);
		ParseMethods(xml.SelectSingleNode("CellMethods"), CellMethods);
		//ParseMethods(xml.SelectSingleNode("BaseMethods"), BaseMethods);
		ParseMethods(xml.SelectSingleNode("ClientMethods"), ClientMethods);
	}

	private void ParseMethods(XmlNode? methodsNode, List<EntityMethodDefinition> methods)
	{
		if (methodsNode is null)
			return;

		foreach (var node in methodsNode.ChildNodes())
			methods.Add(new EntityMethodDefinition(ClientVersion, DefinitionStore, node));

		methods = methods.OrderBy(m => m.DataSize).ToList();
	}
}
