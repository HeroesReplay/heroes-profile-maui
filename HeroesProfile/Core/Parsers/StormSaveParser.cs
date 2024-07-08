using Heroes.StormReplayParser;
using HeroesProfile.UI.Core.Models;

namespace HeroesProfile.UI.Core.Parsers;

public class StormSaveParser : IReplayParser
{
    public ParseType ParseType => ParseType.StormSave;

    public string FileExtension => ".StormSave";

    public async Task<ReplayParseData> ParseAsync(FileInfo file, ParseOptions? options = null, CancellationToken token = default)
    {
        throw new NotImplementedException("");
    }
}