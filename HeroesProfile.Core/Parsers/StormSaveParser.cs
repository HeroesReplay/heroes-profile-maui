using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Foole.Mpq;

using Heroes.ReplayParser;

using MauiApp2.Core.Models;

namespace MauiApp2.Core.Parsers
{
    using MpqAttributeEvents = Heroes.ReplayParser.MPQFiles.ReplayAttributeEvents;
    using MpqDetails = Heroes.ReplayParser.MPQFiles.ReplayDetails;
    using MpqHeader = Heroes.ReplayParser.MPQFiles.MpqHeader;
    using MpqInitData = Heroes.ReplayParser.MPQFiles.ReplayInitData;
    using MpqTrackerEvents = Heroes.ReplayParser.MPQFiles.ReplayTrackerEvents;

    public class StormSaveParser : IReplayParser
    {
        public ParseType ParseType => ParseType.StormSave;

        public string FileExtension => ".StormSave";

        public async Task<ReplayParseData> ParseAsync(FileInfo file, ParseOptions options = null, CancellationToken token = default)
        {
            try
            {
                var bytes = await File.ReadAllBytesAsync(file.FullName, token);

                Replay replay = new Replay();

                MpqHeader.ParseHeader(replay, bytes);

                using (var memoryStream = new MemoryStream(bytes))
                {
                    using (var archive = new MpqArchive(memoryStream, loadListfile: true))
                    {
                        archive.AddListfileFilenames();
                        MpqDetails.Parse(replay, DataParser.GetMpqFile(archive, "save.details"), true);

                        if (archive.FileExists("replay.attributes.events"))
                        {
                            MpqAttributeEvents.Parse(replay, DataParser.GetMpqFile(archive, "replay.attributes.events"));
                        }

                        if (archive.FileExists("save.initData"))
                        {
                            MpqInitData.Parse(replay, DataParser.GetMpqFile(archive, "save.initData"));
                        }

                        if (archive.FileExists("replay.tracker.events"))
                        {
                            replay.TrackerEvents = MpqTrackerEvents.Parse(DataParser.GetMpqFile(archive, "replay.tracker.events"));
                        }
                    }
                }

                return new ReplayParseData()
                {
                    Bytes = bytes,
                    File = file,
                    Replay = replay,
                    ParseResult = ParseResult.Success,
                    Fingerprint = null,
                    ParseType = ParseType
                };
            }
            catch (Exception e)
            {
                // TODO: Logging
                Console.WriteLine(e.Message);
            }

            return new ReplayParseData()
            {
                File = file,
                Replay = new Replay(),
                ParseResult = ParseResult.Exception,
                Fingerprint = null,
                ParseType = ParseType
            };
        }
    }
}