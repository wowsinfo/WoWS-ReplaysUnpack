using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nodsoft.WowsReplaysUnpack.Infrastructure;

internal class DateTimeJsonConverter : JsonConverter<DateTime>
{
	private const string DateFormat = "dd.MM.yyyy HH:mm:ss";
	
	public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return DateTime.ParseExact(reader.GetString()!, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);
	}

	public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.ToString(DateFormat));
	}
}