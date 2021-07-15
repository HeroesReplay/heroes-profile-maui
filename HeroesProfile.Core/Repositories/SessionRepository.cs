using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.Models;
using HeroesProfile.Core.Parsers;

using Polly;

namespace HeroesProfile.Core.Repositories
{
    public class SessionRepository
    {
        private readonly AppSettings appSettings;
        private readonly AggregateReplayParser replayParser;
        private readonly ReaderWriterLockSlim readerWriterLockSlim = new(LockRecursionPolicy.NoRecursion);

        private Session session;

        public Session Session
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
            this.session = new Session();
        }

        public async Task ClearAsync(CancellationToken cancellationToken)
        {
            Session = new Session();

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

        public async Task RefreshAsync(string sessionFile, CancellationToken cancellationToken)
        {
            ReplayParseData parseData = await replayParser.ParseAsync(new FileInfo(sessionFile), cancellationToken);

            switch (parseData.ParseType)
            {
                case ParseType.BattleLobby:
                    {
                        Session.Files.BattleLobby = parseData.Replay;
                        break;
                    }
                case ParseType.StormReplay:
                    {
                        Session.Files.StormReplay = parseData.Replay;
                        break;
                    }
                case ParseType.StormSave:
                    {
                        Session.Files.StormSave = parseData.Replay;
                        break;
                    }
            }
        }
    }
}
