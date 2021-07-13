
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HeroesProfile.Core.JsonConverters
{
    public class FileInfoToFullPathConverter : JsonConverter<FileInfo>
    {
        public override FileInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, FileInfo value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.FullName);
        }
    }
}
