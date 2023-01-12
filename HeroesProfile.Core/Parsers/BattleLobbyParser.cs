using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Heroes.ReplayParser;
using Heroes.ReplayParser.MPQFiles;
using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Parsers;

public class BattleLobbyParser : IReplayParser
{
    public ParseType ParseType => ParseType.BattleLobby;
    public string FileExtension => ".battlelobby";

    public async Task<ReplayParseData> ParseAsync(FileInfo file, ParseOptions options = null, CancellationToken token = default)
    {
        try
        {
            byte[] bytes = await File.ReadAllBytesAsync(file.FullName, token);

            return new ReplayParseData
            {
                Bytes = bytes,
                File = file,
                ParseResult = ParseResult.Success,
                Replay = StandaloneBattleLobbyParser.Parse(bytes),
                Fingerprint = null,
                ParseType = ParseType
            };
        }
        catch
        {
            return new ReplayParseData
            {
                Bytes = null,
                File = file,
                ParseResult = ParseResult.Exception,
                Replay = new Replay(),
                Fingerprint = null,
                ParseType = ParseType
            };
        }
    }
}