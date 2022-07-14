using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

public class MailboxDataType : ADataTypeBase
{
	public MailboxDataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(object))
	{
	}
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	protected override object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
		=> "<Mailbox>";
}
