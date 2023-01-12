using System;

namespace HeroesProfile.Core.Models;

public class StoredReplay : IEquatable<StoredReplay>
{
    public string Path { get; set; }
    public string Fingerprint { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public ParseResult ParseResult { get; set; }
    public ProcessStatus ProcessStatus { get; set; }
    public UploadStatus UploadStatus { get; set; }
    public int? ReplayId { get; set; }

    public StoredReplay()
    {

    }

    public bool Equals(StoredReplay other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Path == other.Path && Created.Equals(other.Created);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((StoredReplay)obj);
    }

    public override int GetHashCode() => HashCode.Combine(Path, Created);
}
