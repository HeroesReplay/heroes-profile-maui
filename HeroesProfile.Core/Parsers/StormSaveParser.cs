using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Heroes.StormReplayParser;

using HeroesProfile.Core.Models;
namespace HeroesProfile.Core.Parsers;


//using Foole.Mpq;
//using Heroes.ReplayParser;
//using MpqAttributeEvents = Heroes.ReplayParser.MPQFiles.ReplayAttributeEvents;
//using MpqDetails = Heroes.ReplayParser.MPQFiles.ReplayDetails;
//using MpqHeader = Heroes.ReplayParser.MPQFiles.MpqHeader;
//using MpqInitData = Heroes.ReplayParser.MPQFiles.ReplayInitData;
//using MpqTrackerEvents = Heroes.ReplayParser.MPQFiles.ReplayTrackerEvents;

public class StormSaveParser : IReplayParser
{
    public ParseType ParseType => ParseType.StormSave;

    public string FileExtension => ".StormSave";

    public async Task<ReplayParseData> ParseAsync(FileInfo file, Heroes.StormReplayParser.ParseOptions options = null, CancellationToken token = default)
    {
        throw new NotImplementedException("");
    }
}