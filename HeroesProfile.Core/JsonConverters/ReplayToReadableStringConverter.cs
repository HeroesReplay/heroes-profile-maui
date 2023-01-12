using Heroes.ReplayParser;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HeroesProfile.Core.JsonConverters;

public class ReplayToReadableStringConverter : JsonConverter<Replay>
{
    public override Replay Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, Replay replay, JsonSerializerOptions options)
    {
        writer.WriteStringValue(replay != null ? $"[{replay.Timestamp}]:{replay.Map}" : string.Empty);
    }
}
