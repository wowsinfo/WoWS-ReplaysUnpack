using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

public abstract class ABaseDefinition
{
	protected Alias Alias { get; }
	protected DefinitionsReader DefinitionsReader { get; }
	public string Name { get; }
	public string Folder { get; }
	public List<PropertyDefinition> Properties { get; } = new();
	public Dictionary<string, object> VolatileProperties { get; } = new();

	public ABaseDefinition(string name, string folder,
		DefinitionsReader definitionsReader, Alias alias)
	{
		Name = name;
		Folder = folder;
		DefinitionsReader = definitionsReader;
		Alias = alias;
		ParseDefinitionFile(DefinitionsReader.GetFileAsXml(Name, Folder).DocumentElement!);
	}

	protected virtual void ParseDefinitionFile(XmlElement xml)
	{
		ParseImplements(xml.SelectNodes("Implements/Interface")!.Cast<XmlNode>().Select(node => node.InnerText.Trim()).ToArray());
		ParseProperties(xml.SelectSingleNode("Properties"));
		ParseVolatile();
	}

	private void ParseImplements(string[] @interfaces)
	{
		foreach (var @interface in @interfaces)
		{
			ParseDefinitionFile(DefinitionsReader.GetFileAsXml(@interface + ".def", Folder, "interfaces").DocumentElement!);
		}
	}

	private void ParseProperties(XmlNode? propertiesNode)
	{
		if (propertiesNode is null)
			return;

		foreach (var propertyNode in propertiesNode.ChildNodes!.Cast<XmlNode>())
		{
			// when same-named properties are in interface and in definition, game client uses last one
			var propertyIndex = Properties.FindIndex(x => x.Name == propertyNode.Name);
			if (propertyIndex > -1)
				Properties.RemoveAt(propertyIndex);

			Properties.Add(new PropertyDefinition(propertyNode, Alias));
		}
	}

	private void ParseVolatile()
	{
		var singleProps = new[] { "yaw", "pitch", "roll" };
		foreach (var property in Properties)
		{
			if (property.Name == "position")
				VolatileProperties[property.Name] = new[] { 0f, 0f, 0f };
			else if (singleProps.Contains(property.Name))
				VolatileProperties[property.Name] = 0.0f;
		}
	}
}
