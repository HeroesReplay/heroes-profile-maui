using Heroes.StormReplayParser;
using HeroesProfile.UI.Core.Models;
// using Heroes.ReplayParser;
// using Heroes.ReplayParser.MPQFiles;

namespace HeroesProfile.UI.Core.Parsers;

public class BattleLobbyParser : IReplayParser
{
    public ParseType ParseType => ParseType.BattleLobby;
    public string FileExtension => ".battlelobby";

    public Task<ReplayParseData> ParseAsync(FileInfo file, ParseOptions? options = null, CancellationToken token = default)
    {
       throw new NotImplementedException();
    }
}