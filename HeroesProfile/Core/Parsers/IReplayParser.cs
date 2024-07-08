using HeroesProfile.UI.Core.Models;

namespace HeroesProfile.UI.Core.Parsers;

public interface IReplayParser
{
    public ParseType ParseType { get; }

    public string FileExtension { get; }

    Task<ReplayParseData> ParseAsync(FileInfo file, Heroes.StormReplayParser.ParseOptions? options = null, CancellationToken token = default);
}