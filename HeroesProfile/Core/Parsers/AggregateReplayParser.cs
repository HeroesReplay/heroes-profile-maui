using Heroes.StormReplayParser;
using HeroesProfile.UI.Core.Models;

namespace HeroesProfile.UI.Core.Parsers;

public class AggregateReplayParser(IEnumerable<IReplayParser> parsers)
{
    public async Task<ReplayParseData> ParseAsync(FileInfo file, ParseOptions? options = null, CancellationToken cancellationToken = default)
    {
        IReplayParser parser = parsers.Single(p => p.FileExtension.Equals(file.Extension, StringComparison.InvariantCultureIgnoreCase));

        return await parser.ParseAsync(file, options, cancellationToken);
    }
}