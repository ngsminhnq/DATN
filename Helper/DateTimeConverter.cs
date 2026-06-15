using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HRemployee.Helper
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly string[] _formats =
        [
            "dd/MM/yyyy HH:mm:ss","dd/MM/yyyy",
            "d/M/yyyy",
            "yyyy-MM-dd",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:ssZ",
            "yyyy-MM-ddTHH:mm:ss.fffZ"
        ];

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString()!;

            if (DateTime.TryParseExact(str, _formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            {
                return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            }

            if (DateTime.TryParse(str, out var fallback))
                return DateTime.SpecifyKind(fallback, DateTimeKind.Utc);

            throw new JsonException($"Không thể đọc ngày '{str}'. Dùng định dạng dd/MM/yyyy (vd: 20/05/1998)");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var format = (value.Hour != 0 || value.Minute != 0 || value.Second != 0) ? "dd/MM/yyyy HH:mm:ss" : "dd/MM/yyyy";
            writer.WriteStringValue(value.ToString(format));
        }
    }

    public class NullableDateTimeConverter : JsonConverter<DateTime?>
    {
        private readonly DateTimeConverter _inner = new();

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            return _inner.Read(ref reader, typeof(DateTime), options);
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value == null) writer.WriteNullValue();
            else _inner.Write(writer, value.Value, options);
        }
    }
}