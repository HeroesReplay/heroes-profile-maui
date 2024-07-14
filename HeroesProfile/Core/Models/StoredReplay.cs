using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Heroes.StormReplayParser;
using Heroes.StormReplayParser.Replay;

namespace HeroesProfile.UI.Core.Models;

public class StoredReplay : IEquatable<StoredReplay>, IEqualityComparer<StoredReplay>, IComparer<StoredReplay>
{
    [JsonPropertyName("Path")]
    public required string Path { get; init; }

    [JsonPropertyName("Created")]
    public required DateTime Created { get; init; }

    [JsonPropertyName("Updated")]
    public required DateTime Updated { get; set; }
    
    [JsonPropertyName("ProcessStatus")]
    public required ProcessStatus ProcessStatus { get; set; } = ProcessStatus.Pending;
    
    [JsonPropertyName("GameMode")]
    public required StormGameMode GameMode { get; set; } = StormGameMode.Unknown;
    
    [JsonPropertyName("ParseStatus")]
    public required StormReplayParseStatus ParseStatus { get; set; } = StormReplayParseStatus.Unknonwn;

    [JsonPropertyName("Fingerprint")]
    public string? Fingerprint { get; set; }
    
    [JsonPropertyName("ReplayId")]
    public long? ReplayId { get; set; }
    
    
    public bool Equals(StoredReplay? other) => other?.Path == Path;

    public static StoredReplay From(ReplayParseData data)
    {
        return new StoredReplay()
        {
            Updated = DateTime.Now,
            ProcessStatus = data.ProcessStatus,
            ParseStatus = data.ParseStatus,
            Created = data.Replay?.Timestamp ?? data.File.CreationTime,
            GameMode = data.Replay?.GameMode ?? StormGameMode.Unknown,
            Path = data.File.FullName,
            Fingerprint = data.Fingerprint,
        };
    }

    public bool Equals(StoredReplay? x, StoredReplay? y) => x?.Path == y?.Path;

    public int GetHashCode([DisallowNull] StoredReplay obj) => obj.Path.GetHashCode();

    public int Compare(StoredReplay? x, StoredReplay? y)
    {
        return x.Created.CompareTo(y.Created);
    }
}