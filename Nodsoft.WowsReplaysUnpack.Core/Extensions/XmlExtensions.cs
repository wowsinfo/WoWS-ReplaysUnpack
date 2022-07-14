using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Extensions;

public static class XmlExtensions
{
	public static string? SelectSingleNodeText(this XmlNode xmlNode, string xPath)
	=> xmlNode.SelectSingleNode(xPath)?.TrimmedText();

	public static string TrimmedText(this XmlNode xmlNode)
	=> xmlNode.InnerText?.Trim() ?? string.Empty;

	public static IEnumerable<XmlNode> SelectXmlNodes(this XmlNode xmlNode, string xPath)
	  => xmlNode.SelectNodes(xPath)?.Cast<XmlNode>() ?? Array.Empty<XmlNode>();

	public static IEnumerable<XmlNode> ChildNodes(this XmlNode xmlNode)
	  => xmlNode.ChildNodes?.Cast<XmlNode>() ?? Array.Empty<XmlNode>();
}
