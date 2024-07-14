using Heroes.StormReplayParser;
using HeroesProfile.UI.Core.Models;
using Microsoft.Extensions.Logging;

namespace HeroesProfile.UI.Core.Parsers;

public class StormSaveParser(ILogger<StormSaveParser> logger) : IReplayParser
{
    public ParseType ParseType => ParseType.StormSave;

    public string FileExtension => ".StormSave";

    public async Task<ReplayParseData> ParseAsync(FileInfo file, CancellationToken token = default)
    {
        try 
        {
            var result = StormReplay.Parse(file.FullName, ParseOptions.MinimalParsing);

            return new ReplayParseData
            {
                File = file,
                ParseStatus = result.Status,
                ProcessStatus = ProcessStatus.Pending,
                ParseType = ParseType.StormSave,
                Replay = result.Replay,
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to parse StormSave file {File}", file.FullName);

            return new ReplayParseData
            {
                File = file,
                ParseStatus = StormReplayParseStatus.Exception,
                ProcessStatus = ProcessStatus.Error,
                ParseType = ParseType.StormSave               
            };
        }
    }
}