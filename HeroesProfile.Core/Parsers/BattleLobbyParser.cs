using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Heroes.StormReplayParser;


// using Heroes.ReplayParser;
// using Heroes.ReplayParser.MPQFiles;
using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Parsers;

public class BattleLobbyParser : IReplayParser
{
    public ParseType ParseType => ParseType.BattleLobby;
    public string FileExtension => ".battlelobby";

    public Task<ReplayParseData> ParseAsync(FileInfo file, ParseOptions? options = null, CancellationToken token = default)
    {
       throw new NotImplementedException();
    }
}