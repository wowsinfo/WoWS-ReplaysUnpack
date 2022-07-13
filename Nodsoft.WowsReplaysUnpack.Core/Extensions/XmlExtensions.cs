using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Extensions;

public static class XmlExtensions
{
	public static string? SelectSingleNodeText(this XmlNode xmlNode, string xPath)
	=> xmlNode.SelectSingleNode(xPath)?.InnerText?.Trim();
}
