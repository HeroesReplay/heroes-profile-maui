
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MauiApp2.Core.JsonConverters
{
    public class ByteArrayToReadableStringConverter : JsonConverter<byte[]>
    {
        public const long OneKB = 1024;

        public const long OneMB = OneKB * OneKB;

        public const long OneGB = OneMB * OneKB;

        public const long OneTB = OneGB * OneKB;

        public static string BytesToHumanReadable(long bytes) => bytes switch
        {
            < OneKB => $"{bytes}B",
            >= OneKB and < OneMB => $"{bytes / OneKB}KB",
            >= OneMB and < OneGB => $"{bytes / OneMB}MB",
            >= OneGB and < OneTB => $"{bytes / OneMB}GB",
            >= OneTB => $"{bytes / OneTB}"
        };

        public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value != null ? BytesToHumanReadable(value.LongLength) : "null");
        }
    }
}
