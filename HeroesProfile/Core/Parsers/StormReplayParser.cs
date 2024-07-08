using System.Security.Cryptography;
using System.Text;
using Heroes.StormReplayParser;
using Heroes.StormReplayParser.Replay;
using HeroesProfile.UI.Core.Models;

namespace HeroesProfile.UI.Core.Parsers;

public class StormReplayParser : IReplayParser
{
    public ParseType ParseType => ParseType.StormReplay;

    public string FileExtension => ".StormReplay";

    private readonly StormGameMode[] SupportedModes =
    {
        StormGameMode.QuickMatch,
        StormGameMode.UnrankedDraft,
        StormGameMode.StormLeague,
    };

    private readonly StormReplayParseStatus[] NotSupportedStatus =
    [
        StormReplayParseStatus.FileNotFound,
        StormReplayParseStatus.Unknonwn,
        StormReplayParseStatus.UnexpectedResult,
        StormReplayParseStatus.TryMeMode,
        StormReplayParseStatus.PreAlphaWipe,
        StormReplayParseStatus.PTRRegion,
        StormReplayParseStatus.Incomplete,
        StormReplayParseStatus.FileSizeTooLarge
    ];

    private string? GetFingerprint(StormReplay? replay)
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
            var result = StormReplay.Parse(file.FullName, options ?? ParseOptions.MinimalParsing);

            if (result.Exception != null)
            {
                return new ReplayParseData()
                {
                    File = file,
                    Replay = result.Replay,
                    ProcessStatus = ProcessStatus.NotSupported,
                    ParseStatus = result.Status,
                    ParseType = ParseType
                };
            }

            if (NotSupportedStatus.Contains(result.Status))
            {
                return new ReplayParseData()
                {
                    File = file,
                    Replay = result.Replay,
                    ProcessStatus = ProcessStatus.NotSupported,
                    ParseStatus = result.Status,
                    ParseType = ParseType
                };
            }

            if (result.Status == StormReplayParseStatus.Success && SupportedModes.Contains(result.Replay.GameMode))
            {
                return new ReplayParseData()
                {
                    File = file,
                    Replay = result.Replay,
                    ProcessStatus = ProcessStatus.Pending,
                    ParseStatus = result.Status,
                    Fingerprint = GetFingerprint(result.Replay),
                    ParseType = ParseType
                };
            }

            return new ReplayParseData()
            {
                File = file,
                Replay = result.Replay,
                ProcessStatus = ProcessStatus.NotSupported,
                ParseStatus = result.Status,
                ParseType = ParseType
            };
        }
        catch (Exception)
        {
            return new ReplayParseData()
            {
                File = file,
                ProcessStatus = ProcessStatus.Error,
                ParseStatus = StormReplayParseStatus.UnexpectedResult,
                ParseType = ParseType
            };
        }
    }
}