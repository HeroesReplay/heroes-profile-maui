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
        var result = Heroes.StormReplayParser.StormReplayPregame.Parse(file.FullName);

        var status = result.Status switch {
            StormReplayPregameParseStatus.Success => StormReplayParseStatus.Success,
            StormReplayPregameParseStatus.PTRRegion => StormReplayParseStatus.PTRRegion,
            StormReplayPregameParseStatus.Unknown => StormReplayParseStatus.Unknonwn,            
            _ => StormReplayParseStatus.Unknonwn
        };
 
        return Task.FromResult(new ReplayParseData
        {
            File = file,
            ParseStatus = status,
            ProcessStatus = ProcessStatus.Pending,
            ParseType = ParseType.BattleLobby
        });
    }
}