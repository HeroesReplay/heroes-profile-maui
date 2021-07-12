using Heroes.ReplayParser;
using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Services
{
    public class SessionManager
    {
        public Session Session { get; private set; }

        public void SetBattleLobby(Replay replay)
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
        }

        public void SetStormSave(Replay replay)
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
        }

        public void SetStormReplay(Replay replay)
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
        }
    }
}
