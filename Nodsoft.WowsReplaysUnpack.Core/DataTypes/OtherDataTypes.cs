using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Text;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

public class BlobDataType : ADataTypeBase
{
	protected override object? GetValue(BinaryReader reader)
		=> reader.ReadBytes(GetActualDataSize(reader));
}

public class StringDataType : ADataTypeBase
{
	protected override object? GetValue(BinaryReader reader)
		=> Encoding.UTF8.GetString(reader.ReadBytes(GetActualDataSize(reader)));
}

public class UnicodeStringDataType : ADataTypeBase
{
	protected override object? GetValue(BinaryReader reader)
		=> Encoding.Unicode.GetString(reader.ReadBytes(GetActualDataSize(reader)));
}

public class MailboxDataType : ADataTypeBase
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	protected  override object? GetValue(BinaryReader reader)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
		=> "<Mailbox>";
}
