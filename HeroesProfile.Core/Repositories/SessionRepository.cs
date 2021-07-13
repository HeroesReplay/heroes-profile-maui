using System;

using Heroes.ReplayParser;

using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Repositories
{
    public class SessionRepository
    {
        private Session Session { get; set; }

        private static readonly object SyncLock = new();

        public Session GetSession() => Session;

        public Session Set(Session session)
        {
            lock (SyncLock)
            {
                Session = session;
            }

            return Session;
        }

        public Session SetBattleLobby(string replayId, Replay replay)
        {
            lock (SyncLock)
            {
                Session = new Session()
                {
                    ReplayId = replayId,
                    BattleLobby = replay,
                    PlayerFoundTalents = new(),
                    StormReplay = null,
                    StormSave = null,
                    TrackerEventIndex = 0,
                    TwitchExtensionId = null,
                    TwitchPredictionId = null,
                    TwitchPredictionWinningOutcomeId = null,
                    State = SessionState.BattleLobby,
                    GameModeUpdated = false,
                    TalentsUpdated = false
                };

                return Session;
            }
        }

        public Session SetStormSave(Replay replay)
        {
            lock (SyncLock)
            {
                Session = new Session()
                {
                    ReplayId = Session.ReplayId,
                    BattleLobby = Session.BattleLobby,
                    PlayerFoundTalents = Session.PlayerFoundTalents,
                    StormReplay = Session.StormReplay,
                    StormSave = replay,
                    TrackerEventIndex = Session.TrackerEventIndex,
                    TwitchExtensionId = Session.TwitchExtensionId,
                    TwitchPredictionId = Session.TwitchPredictionId,
                    TwitchPredictionWinningOutcomeId = Session.TwitchPredictionWinningOutcomeId,
                    State = SessionState.StormSave,
                    GameModeUpdated = false,
                    TalentsUpdated = false
                };

                return Session;
            }
        }

        public Session SetStormReplay(Replay replay)
        {
            lock (SyncLock)
            {
                Session = new Session()
                {
                    BattleLobby = Session.BattleLobby,
                    PlayerFoundTalents = Session.PlayerFoundTalents,
                    StormReplay = replay,
                    StormSave = Session.StormSave,
                    TrackerEventIndex = Session.TrackerEventIndex,
                    TwitchExtensionId = Session.TwitchExtensionId,
                    TwitchPredictionId = Session.TwitchPredictionId,
                    TwitchPredictionWinningOutcomeId = Session.TwitchPredictionWinningOutcomeId,
                    State = SessionState.StormReplay,
                    GameModeUpdated = false,
                    TalentsUpdated = false
                };

                return Session;
            }
        }
    }
}
