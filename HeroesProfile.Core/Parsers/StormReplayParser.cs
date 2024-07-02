using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Heroes.ReplayParser;
using Heroes.StormReplayParser;
using Heroes.StormReplayParser.Replay;

using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Parsers;

public class StormReplayParser : IReplayParser
{
    public ParseType ParseType => ParseType.StormReplay;

    public string FileExtension => ".StormReplay";

    private string? GetFingerprint(StormReplay replay)
    {
        if (replay == null) return null;

        try
        {
            using (var md5 = MD5.Create())
            {
                var battleNetIds = string.Join(string.Empty, replay.StormPlayers.Select(x => x.ToonHandle!.Id).OrderBy(x => x));
                return new Guid(md5.ComputeHash(Encoding.UTF8.GetBytes(string.Join(string.Empty, battleNetIds, replay.RandomValue)))).ToString();
            }
        }
        catch
        {
            return null;
        }
    }

    public async Task<ReplayParseData> ParseAsync(FileInfo file, ParseOptions? options = null, CancellationToken token = default)
    {
        try
        {
            // byte[] bytes = await File.ReadAllBytesAsync(file.FullName, token);
            var result = StormReplay.Parse(file.FullName, options ?? ParseOptions.MinimalParsing);

            var status = result.Status;
            var replay = result.Replay;

            if (status == StormReplayParseStatus.Success)
            {
                var supported = new StormGameMode[] { StormGameMode.ARAM, StormGameMode.QuickMatch, StormGameMode.StormLeague, StormGameMode.UnrankedDraft };

                if (supported.Contains(result.Replay.GameMode))
                {
                    return new ReplayParseData()
                    {
                        //Bytes = bytes,
                        File = file,
                        Replay = replay,
                        ParseResult = ParseResult.Success,
                        Fingerprint = GetFingerprint(replay),
                        ParseType = ParseType
                    };
                }

                return new ReplayParseData()
                {
                    //Bytes = bytes,
                    File = file,
                    Replay = replay,
                    ParseResult = ParseResult.UnexpectedResult,
                    Fingerprint = null,
                    ParseType = ParseType
                };
            }

            //string replayParseResult = Enum.GetName(typeof(DataParser.ReplayParseResult), status);

            if (result != null)
            {
                //var parseResult = Enum.Parse<ParseResult>(result.Status, ignoreCase: true);

                return new ReplayParseData()
                {
                    //Bytes = bytes,
                    File = file,
                    Replay = replay,
                    ParseResult = (ParseResult)result.Status,
                    Fingerprint = null,
                    ParseType = ParseType
                };
            }
            else
            {
                return new ReplayParseData()
                {
                    //Bytes = bytes,
                    File = file,
                    //Replay = new Replay(),
                    ParseResult = ParseResult.UnexpectedResult,
                    Fingerprint = null,
                    ParseType = ParseType
                };
            }
        }
        catch (Exception)
        {
            return new ReplayParseData()
            {
                //Bytes = null,
                File = file,
                //Replay = new Replay(),
                ParseResult = ParseResult.Exception,
                Fingerprint = null,
                ParseType = ParseType
            };
        }
    }
}