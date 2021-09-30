using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Heroes.ReplayParser;

using MauiApp2.Core.Models;

namespace MauiApp2.Core.Parsers
{
    public class StormReplayParser : IReplayParser
    {
        public ParseType ParseType => ParseType.StormReplay;

        public string FileExtension => ".StormReplay";

        private string GetFingerprint(Replay replay)
        {
            if (replay == null) return null;

            try
            {
                using (var md5 = MD5.Create())
                {
                    var battleNetIds = string.Join(string.Empty, replay.Players.Select(x => x.BattleNetId).OrderBy(x => x));
                    return new Guid(md5.ComputeHash(Encoding.UTF8.GetBytes(string.Join(string.Empty, battleNetIds, replay.RandomValue)))).ToString();
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<ReplayParseData> ParseAsync(FileInfo file, ParseOptions options = null, CancellationToken token = default)
        {
            try
            {
                byte[] bytes = await File.ReadAllBytesAsync(file.FullName, token);
                var (status, replay) = DataParser.ParseReplay(bytes, options ?? ParseOptions.DefaultParsing);

                if (status == DataParser.ReplayParseResult.Success)
                {
                    var supported = new GameMode[] { GameMode.ARAM, GameMode.QuickMatch, GameMode.StormLeague, GameMode.UnrankedDraft };

                    if (supported.Contains(replay.GameMode))
                    {
                        return new ReplayParseData()
                        {
                            Bytes = bytes,
                            File = file,
                            Replay = replay,
                            ParseResult = ParseResult.Success,
                            Fingerprint = GetFingerprint(replay),
                            ParseType = ParseType
                        };
                    }

                    return new ReplayParseData()
                    {
                        Bytes = bytes,
                        File = file,
                        Replay = replay,
                        ParseResult = ParseResult.CustomGame,
                        Fingerprint = null,
                        ParseType = ParseType
                    };
                }

                string replayParseResult = Enum.GetName(typeof(DataParser.ReplayParseResult), status);

                if (!string.IsNullOrWhiteSpace(replayParseResult))
                {
                    var parseResult = Enum.Parse<ParseResult>(replayParseResult, ignoreCase: true);

                    return new ReplayParseData()
                    {
                        Bytes = bytes,
                        File = file,
                        Replay = replay,
                        ParseResult = parseResult,
                        Fingerprint = null,
                        ParseType = ParseType
                    };
                }
                else
                {
                    return new ReplayParseData()
                    {
                        Bytes = bytes,
                        File = file,
                        Replay = new Replay(),
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
                    Bytes = null,
                    File = file,
                    Replay = new Replay(),
                    ParseResult = ParseResult.Exception,
                    Fingerprint = null,
                    ParseType = ParseType
                };
            }
        }
    }
}