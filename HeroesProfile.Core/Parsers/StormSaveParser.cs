using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Heroes.StormReplayParser;

using HeroesProfile.Core.Models;
namespace HeroesProfile.Core.Parsers;

public class StormSaveParser : IReplayParser
{
    public ParseType ParseType => ParseType.StormSave;

    public string FileExtension => ".StormSave";

    public async Task<ReplayParseData> ParseAsync(FileInfo file, ParseOptions? options = null, CancellationToken token = default)
    {
        throw new NotImplementedException("");
    }
}