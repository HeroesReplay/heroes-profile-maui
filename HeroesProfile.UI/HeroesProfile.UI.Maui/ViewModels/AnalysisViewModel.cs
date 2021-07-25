using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;

using MediatR;

using ReactiveUI;

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HeroesProfile.UI.Maui.ViewModels
{
    public class AnalysisViewModel : ReactiveObject
    {
        private readonly IMediator mediator;

        public bool HasBattleLobby => Session.BattleLobby != null;
        public bool HasStormSave => Session.StormSave != null;
        public bool HasStormReplay => Session.StormReplay != null;
        public bool HasPreMatch => session.PreMatchUri != null;
        public bool HasPostMatch => session.PostMatchUri != null;


        private SessionData session;
        private UserSettings settings;
        private IEnumerable<int> battlenetIds;

        public IEnumerable<int> BattlenetIds
        {
            get => battlenetIds;
            set
            {
                battlenetIds = value;
                this.RaisePropertyChanged();
            }
        }

        public SessionData Session
        {
            get => session;
            set
            {
                session = value;
                this.RaisePropertyChanged();
            }
        }

        public UserSettings UserSettings
        {
            get => settings;
            set
            {
                settings = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsTwitchTalentsEnabled => UserSettings != null && UserSettings.EnableTalentsExtension;
        public bool IsTwitchPredictionsEnabled => UserSettings != null && UserSettings.EnablePredictions;

        public bool IsPostMatchEnabled => UserSettings != null && UserSettings.EnablePostMatch;
        public bool IsPreMatchEnabled => UserSettings != null && UserSettings.EnablePreMatch;


        public void OpenInBrowser(string uri)
        {
            if (System.OperatingSystem.IsWindows())
            {
                using (Process proc = new Process())
                {
                    proc.StartInfo.UseShellExecute = true;
                    proc.StartInfo.FileName = uri;
                    proc.Start();
                }
            }
            else if (System.OperatingSystem.IsMacCatalyst())
            {

            }
        }

        public AnalysisViewModel(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task LoadAsync()
        {
            GetSession.Response sessionResponse = await this.mediator.Send(new GetSession.Query());
            GetUserSettings.Response settingsResponse = await this.mediator.Send(new GetUserSettings.Query());
            GetKnownBattleNetIds.Response battleNetResponse = await this.mediator.Send(new GetKnownBattleNetIds.Query());

            Session = sessionResponse.Session;
            UserSettings = settingsResponse.UserSettings;
            BattlenetIds = battleNetResponse.BattleNetIds;
        }
    }
}
