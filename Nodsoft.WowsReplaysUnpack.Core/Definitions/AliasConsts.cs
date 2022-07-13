using Nodsoft.WowsReplaysUnpack.Core.DataTypes;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

public static class AliasConsts
{
	public static Dictionary<string, Type> SimpleTypeMappings = new()
	{
		{ "BLOB", typeof(BlobDataType) },
		{ "STRING", typeof(StringDataType) },
		{ "UNICODE_STRING", typeof(StringDataType) },
		{ "FLOAT", typeof(Float32DataType) },
		{ "FLOAT32", typeof(Float32DataType) },
		{ "FLOAT64", typeof(Float64DataType) },
		{ "INT8", typeof(Int8DataType) },
		{ "INT16", typeof(Int16DataType) },
		{ "INT32", typeof(Int32DataType) },
		{ "INT64", typeof(Int64DataType) },
		{ "UINT8", typeof(UInt8DataType) },
		{ "UINT16", typeof(UInt16DataType) },
		{ "UINT32", typeof(UInt32DataType) },
		{ "UINT64", typeof(UInt64DataType) },
		{ "VECTOR2", typeof(Vector2DataType) },
		{ "VECTOR3", typeof(Vector3DataType) },
		{ "VECTOR4", typeof(Vector4DataType) },
		//{ "MAILBOX", typeof(double) },
		//{ "PYTHON", typeof(double) },
		//{ "FIXED_DICT", typeof(double) },
		//{ "ARRAY", typeof(Array) },
		//{ "TUPLE", typeof(Array) },
		//{ "USER_TYPE", typeof(double) }
	};

	//private readonly Dictionary<string, string> _mappings = new();

	//public AliasConsts(DefinitionsReader definitionsReader)
	//{
	//	var aliasXml = definitionsReader.GetFileAsXml("alias.xml", "entity_defs");
	//	foreach (XmlNode node in aliasXml.DocumentElement!.ChildNodes)
	//	{
	//		_mappings[node.Name] = node.InnerText.Trim();
	//	}
	//}

	//public ADataTypeBase GetDataType(string dataTypeName)
	//{
	//	if (_mappings.TryGetValue(dataTypeName, out var mapping))
	//		return GetDataType(mapping);
	//	else if (_simpleTypes.TryGetValue(dataTypeName, out var dataType))
	//		return dataType;
	//	else
	//		throw new NotSupportedException($"DataType {dataTypeName} is not supported");
	//}
}
