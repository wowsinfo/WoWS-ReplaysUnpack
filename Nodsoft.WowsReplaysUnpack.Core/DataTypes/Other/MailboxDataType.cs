using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

public class MailboxDataType : DataTypeBase
{
	public MailboxDataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode) : base(version, definitionStore, xmlNode, typeof(object))
	{
	}
	
	protected override object GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize) => "<Mailbox>";
}
