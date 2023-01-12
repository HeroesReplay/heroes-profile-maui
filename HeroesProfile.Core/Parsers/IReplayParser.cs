using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Heroes.ReplayParser;
using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Parsers;

public interface IReplayParser
{
    public ParseType ParseType { get; }

    public string FileExtension { get; }

    Task<ReplayParseData> ParseAsync(FileInfo file, ParseOptions options = null, CancellationToken token = default);
}