using System.IO;
using System.Text.Json.Serialization;

using Heroes.ReplayParser;

namespace HeroesProfile.Core.Models;

public class ReplayParseData
{
    public byte[] Bytes { get; init; }
    public FileInfo File { get; init; }
    public ParseType ParseType { get; init; }
    public Replay Replay { get; init; }
    public ParseResult ParseResult { get; init; }
    public string Fingerprint { get; init; }
}