using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

public class EntityDefinition : ABaseDefinition
{
	public List<EntityMethodDefinition> CellMethods { get; set; } = new();
	public List<EntityMethodDefinition> BaseMethods { get; set; } = new();
	public List<EntityMethodDefinition> ClientMethods { get; set; } = new();
	public EntityDefinition(string name, string folder, DefinitionsReader definitionsReader, Alias alias)
		: base(name, folder, definitionsReader, alias)
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

		foreach (var node in methodsNode.ChildNodes.Cast<XmlNode>())
			methods.Add(new EntityMethodDefinition(node, Alias));
	}
}
