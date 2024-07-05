using Heroes.StormReplayParser;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HeroesProfile.Core.JsonConverters;

public class ReplayToReadableStringConverter : JsonConverter<StormReplay>
{
    public override StormReplay Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, StormReplay replay, JsonSerializerOptions options)
    {
        writer.WriteStringValue(replay != null ? $"[{replay.Timestamp}]:{replay.MapInfo.MapName}" : string.Empty);
    }
}
