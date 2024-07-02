using System.IO;
using System.Text.Json.Serialization;

using Heroes.ReplayParser;
using Heroes.StormReplayParser;

namespace HeroesProfile.Core.Models;

public class ReplayParseData
{    
    public FileInfo File { get; init; }
    public ParseType ParseType { get; init; }
    public StormReplay? Replay { get; init; }
    public ParseResult ParseResult { get; init; }
    public string? Fingerprint { get; init; }
}