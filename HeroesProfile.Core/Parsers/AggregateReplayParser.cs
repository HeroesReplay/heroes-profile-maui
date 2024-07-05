using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Heroes.StormReplayParser;
using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Parsers;

public class AggregateReplayParser(IEnumerable<IReplayParser> parsers)
{
    public async Task<ReplayParseData> ParseAsync(FileInfo file, ParseOptions? options = null, CancellationToken cancellationToken = default)
    {
        IReplayParser parser = parsers.Single(p => p.FileExtension.Equals(file.Extension, StringComparison.InvariantCultureIgnoreCase));

        return await parser.ParseAsync(file, options, cancellationToken);
    }
}