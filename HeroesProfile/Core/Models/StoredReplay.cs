using Heroes.StormReplayParser;

namespace HeroesProfile.UI.Core.Models;

public class StoredReplay : IEquatable<StoredReplay>
{
    /// <summary>
    /// We need the full path to the replay
    /// </summary>
    public required string Path { get; init; }

    /// <summary>
    /// The calculated fingerprint, null if no replay was parsed
    /// </summary>
    public string? Fingerprint { get; init; }

    /// <summary>
    /// The time the file itself was created, regardless of the replay
    /// This should be the replay metadata creation time if the replay file was parsed successfully.
    /// </summary>
    public required DateTime Created { get; init; }

    /// <summary>
    /// The time that the uploader did anything processing this specific file, uploading etc
    /// </summary>
    public required DateTime Updated { get; set; }

    /// <summary>
    /// The status of the processing of the replay file
    /// Once a file is created, it should default to Pending
    /// Pending - The file has not been processed and decided what to do with it
    /// </summary>
    public required ProcessStatus ProcessStatus { get; set; } = ProcessStatus.Pending;

    /// <summary>
    /// The status of the parsing of the replay file
    /// Unknown should be the default value until it has attempted to parse the file
    /// </summary>
    public required StormReplayParseStatus? ParseStatus { get; set; } = StormReplayParseStatus.Unknonwn;

    /// <summary>
    /// When a replay is uploaded, the id of the replay is stored here
    /// </summary>
    public long? ReplayId { get; set; }

    public bool Equals(StoredReplay? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Path == other.Path && Created.Equals(other.Created);
    }
}