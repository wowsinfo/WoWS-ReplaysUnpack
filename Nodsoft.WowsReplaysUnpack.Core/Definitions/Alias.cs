using Nodsoft.WowsReplaysUnpack.Core.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

public class Alias
{
	private readonly Dictionary<string, ADataTypeBase> _simpleTypes = new()
	{
		{ "BLOB", new BlobDataType() },
		{ "STRING", new StringDataType() },
		{ "UNICODE_STRING", new StringDataType() },
		{ "FLOAT", new Float32DataType() },
		{ "FLOAT32", new Float32DataType() },
		{ "FLOAT64", new Float64DataType() },
		{ "INT8", new Int8DataType() },
		{ "INT16", new Int16DataType() },
		{ "INT32", new Int32DataType() },
		{ "INT64", new Int64DataType() },
		{ "UINT8", new UInt8DataType() },
		{ "UINT16", new UInt16DataType() },
		{ "UINT32", new UInt32DataType() },
		{ "UINT64", new UInt64DataType() },
		{ "VECTOR2", new Vector2DataType() },
		{ "VECTOR3", new Vector3DataType() },
		{ "VECTOR4", new Vector4DataType() },
		//{ "MAILBOX", typeof(double) },
		//{ "PYTHON", typeof(double) },
		//{ "FIXED_DICT", typeof(double) },
		//{ "ARRAY", typeof(Array) },
		//{ "TUPLE", typeof(Array) },
		//{ "USER_TYPE", typeof(double) }
	};

	private readonly Dictionary<string, string> _mappings = new();

	public Alias(DefinitionsReader definitionsReader)
	{
		var aliasXml = definitionsReader.GetFileAsXml("alias.xml", "entity_defs");
		foreach (XmlNode node in aliasXml.DocumentElement!.ChildNodes)
		{
			_mappings[node.Name] = node.InnerText.Trim();
		}
	}

	public ADataTypeBase GetDataType(string dataTypeName)
	{
		if (_mappings.TryGetValue(dataTypeName, out var mapping))
			return GetDataType(mapping);
		else if (_simpleTypes.TryGetValue(dataTypeName, out var dataType))
			return dataType;
		else
			throw new NotSupportedException($"DataType {dataTypeName} is not supported");
	}
}
