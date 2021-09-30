using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Heroes.ReplayParser;

using MauiApp2.Core.Models;

namespace MauiApp2.Core.Parsers
{
    public interface IReplayParser
    {
        public ParseType ParseType { get; }

        public string FileExtension { get; }

        Task<ReplayParseData> ParseAsync(FileInfo file, ParseOptions options = null, CancellationToken token = default);
    }
}