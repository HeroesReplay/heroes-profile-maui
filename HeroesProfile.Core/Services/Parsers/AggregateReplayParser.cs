using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Services.Parsers
{
    public class AggregateReplayParser
    {
        private readonly IEnumerable<IReplayParser> parsers;

        public AggregateReplayParser(IEnumerable<IReplayParser> parsers)
        {
            this.parsers = parsers;
        }

        public async Task<ReplayParseData> ParseAsync(FileInfo file, CancellationToken cancellationToken)
        {
            IReplayParser parser = parsers.Single(p => p.FileExtension.Equals(file.Extension, StringComparison.InvariantCultureIgnoreCase));

            return await parser.ParseAsync(file, cancellationToken);
        }
    }
}