using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Heroes.ReplayParser;

using MauiApp2.Core.Models;
using MauiApp2.Core.Parsers;

using Polly;

namespace MauiApp2.Core.Repositories
{
    public class SessionRepository
    {
        private readonly AppSettings appSettings;
        private readonly AggregateReplayParser replayParser;
        private readonly ReaderWriterLockSlim readerWriterLockSlim = new(LockRecursionPolicy.NoRecursion);

        private SessionData session;

        public SessionData SessionData
        {
            get
            {
                try
                {
                    readerWriterLockSlim.EnterReadLock();
                    return session;
                }
                finally
                {
                    readerWriterLockSlim.ExitReadLock();
                }
            }
            set
            {
                try
                {
                    readerWriterLockSlim.EnterWriteLock();
                    session = value;
                }
                finally
                {
                    readerWriterLockSlim.ExitWriteLock();
                }
            }
        }

        public SessionRepository(AppSettings appSettings, AggregateReplayParser replayParser)
        {
            this.appSettings = appSettings;
            this.replayParser = replayParser;
            session = new SessionData();
        }

        public async Task ClearAsync(CancellationToken cancellationToken)
        {
            SessionData = new SessionData();

            await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(Enumerable.Range(1, 5).Select(x => TimeSpan.FromSeconds(x)))
                .ExecuteAsync(async (token) =>
                {
                    await Task.Run(() =>
                    {
                        foreach (var file in new DirectoryInfo(appSettings.ApplicationSessionDirectory).EnumerateFiles("*.*", SearchOption.AllDirectories))
                        {
                            file.Delete();
                        }
                    }, token);

                }, cancellationToken);
        }

        public async Task UpdateAsync(string sessionFile, CancellationToken cancellationToken)
        {
            var options = new ParseOptions
            {
                ShouldParseMessageEvents = false,
                ShouldParseMouseEvents = false,
                ShouldParseUnits = false,
                AllowPTR = false,
                IgnoreErrors = true,
                ShouldParseDetailedBattleLobby = true,
                ShouldParseStatistics = false,
                ShouldParseEvents = true
            };

            ReplayParseData parseData = await replayParser.ParseAsync(new FileInfo(sessionFile), options, cancellationToken);

            switch (parseData.ParseType)
            {
                case ParseType.BattleLobby:
                    {
                        SessionData.Files.BattleLobby = new SessionFile(parseData.Replay, parseData.ParseType, DateTime.Now);
                        break;
                    }
                case ParseType.StormReplay:
                    {
                        SessionData.Files.StormReplay = new SessionFile(parseData.Replay, parseData.ParseType, DateTime.Now);
                        break;
                    }
                case ParseType.StormSave:
                    {
                        SessionData.Files.StormSave = new SessionFile(parseData.Replay, parseData.ParseType, DateTime.Now);
                        break;
                    }
            }
        }
    }
}
