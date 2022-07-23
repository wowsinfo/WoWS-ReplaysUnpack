using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Extensions;

/// <summary>
/// Various extension methods for working with XML.
/// </summary>
public static class XmlExtensions
{
	/// <summary>
	/// Selects a single node from the XML document, at the given XPath.
	/// </summary>
	/// <param name="xmlNode">The XML document.</param>
	/// <param name="xPath">The XPath to the node.</param>
	/// <returns>The node value, or null if not found.</returns>
	public static string? SelectSingleNodeText(this XmlNode xmlNode, string xPath) => xmlNode.SelectSingleNode(xPath)?.TrimmedText();

	/// <summary>
	/// Returns a trimmed version of the text in the given XML node.
	/// </summary>
	/// <param name="xmlNode">The XML node.</param>
	/// <returns>The trimmed text.</returns>
	public static string TrimmedText(this XmlNode xmlNode) => xmlNode.InnerText.Trim();

	/// <summary>
	/// Enumerates the nodes to a given relative XPath of a XML node.
	/// </summary>
	/// <param name="xmlNode">The base XML node.</param>
	/// <param name="xPath">The relative XPath to the nodes.</param>
	/// <returns>The child nodes.</returns>
	public static IEnumerable<XmlNode> SelectXmlNodes(this XmlNode xmlNode, string xPath) => xmlNode.SelectNodes(xPath)?.Cast<XmlNode>() ?? Array.Empty<XmlNode>();

	/// <summary>
	/// Enumerates a XML node's child nodes.
	/// </summary>
	/// <param name="xmlNode">The base XML node.</param>
	/// <returns>The child nodes.</returns>
	public static IEnumerable<XmlNode> ChildNodes(this XmlNode xmlNode) => xmlNode.ChildNodes.Cast<XmlNode>();
}
