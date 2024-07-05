using System.IO;
using Heroes.StormReplayParser;

namespace HeroesProfile.Core.Models;

public class ReplayParseData
{
    public required FileInfo File { get; init; }
    public ParseType? ParseType { get; init; }
    public StormReplay? Replay { get; init; } = null;
    public StormReplayParseStatus ParseStatus { get; init; } = StormReplayParseStatus.Unknonwn;
    public ProcessStatus ProcessStatus { get; init; } = ProcessStatus.Pending;
    public string? Fingerprint { get; init; }
}