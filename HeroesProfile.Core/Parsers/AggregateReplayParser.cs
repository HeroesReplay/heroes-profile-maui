using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Heroes.ReplayParser;

using MauiApp2.Core.Models;

namespace MauiApp2.Core.Parsers
{
    public class AggregateReplayParser
    {
        private readonly IEnumerable<IReplayParser> parsers;

        public AggregateReplayParser(IEnumerable<IReplayParser> parsers)
        {
            this.parsers = parsers;
        }

        public async Task<ReplayParseData> ParseAsync(FileInfo file, ParseOptions options = null, CancellationToken cancellationToken = default)
        {
            IReplayParser parser = parsers.Single(p => p.FileExtension.Equals(file.Extension, StringComparison.InvariantCultureIgnoreCase));

            return await parser.ParseAsync(file, options, cancellationToken);
        }
    }
}