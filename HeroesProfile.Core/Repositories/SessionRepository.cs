using Heroes.ReplayParser;
using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Repositories
{
    public class SessionRepository
    {
        public Session Session { get; private set; }

        public void Set(Replay replay, ParseType parseType)
        {
            _ = parseType switch
            {
                ParseType.BattleLobby => SetBattleLobby(replay),
                ParseType.StormReplay => SetStormReplay(replay),
                ParseType.StormSave => SetStormSave(replay),
                ParseType.All => throw new System.NotImplementedException(),
                _ => throw new System.NotImplementedException(),
            };
        }

        public Session SetBattleLobby(Replay replay)
        {
            Session = new Session()
            {
                BattleLobby = replay,
                PlayerFoundTalents = new(),
                StormReplay = null,
                StormSave = null,
                TrackerEventIndex = 0,
                TwitchExtensionId = null,
                TwitchPredictionId = null,
                TwitchPredictionWinningOutcomeId = null,
                State = SessionState.BattleLobby,
                UpdatedMode = false,
                UpdatedTalents = false
            };

            return Session;
        }

        public Session SetStormSave(Replay replay)
        {
            Session = new Session()
            {
                BattleLobby = Session.BattleLobby,
                PlayerFoundTalents = Session.PlayerFoundTalents,
                StormReplay = Session.StormReplay,
                StormSave = replay,
                TrackerEventIndex = Session.TrackerEventIndex,
                TwitchExtensionId = Session.TwitchExtensionId,
                TwitchPredictionId = Session.TwitchPredictionId,
                TwitchPredictionWinningOutcomeId = Session.TwitchPredictionWinningOutcomeId,
                State = SessionState.StormSave,
                UpdatedMode = false,
                UpdatedTalents = false
            };

            return Session;
        }

        public Session SetStormReplay(Replay replay)
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
                UpdatedMode = false,
                UpdatedTalents = false
            };

            return Session;
        }
    }
}
