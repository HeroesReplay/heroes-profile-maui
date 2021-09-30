
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MauiApp2.Core.JsonConverters
{
    public class FileInfoToFullPathConverter : JsonConverter<FileInfo>
    {
        public override FileInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            else
            {
                string path = reader.GetString();
                if (!string.IsNullOrWhiteSpace(path)) return new FileInfo(path);
                else return null;
            }
        }

        public override void Write(Utf8JsonWriter writer, FileInfo value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.FullName);
        }
    }
}
