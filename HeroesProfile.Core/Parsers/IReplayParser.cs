using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Parsers
{
    public interface IReplayParser
    {
        public ParseType ParseType { get; }

        public string FileExtension { get; }

        Task<ReplayParseData> ParseAsync(FileInfo file, CancellationToken token);
    }
}