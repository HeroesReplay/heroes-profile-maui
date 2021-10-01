using Blazorise.Localization;
using Blazorise;

using MediatR;

using Microsoft.AspNetCore.Components;

using ReactiveUI;

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MauiApp2.Core.CQRS.Queries;
using MauiApp2.Core.Models;
using System.Linq;

namespace MauiApp2.ViewModels
{

    public class AnalysisViewModel : ReactiveObject
    {
        private readonly IMediator mediator;

        public bool HasBattleLobby => Session?.BattleLobby != null;
        public bool HasStormSave => Session?.StormSave != null;
        public bool HasStormReplay => Session?.StormReplay != null;
        public bool HasPreMatch => session?.PreMatchUri != null;
        public bool HasPostMatch => session?.PostMatchUri != null;


        private SessionData session;
        private UserSettings settings;
        private IEnumerable<int> battlenetIds;

        public IEnumerable<int> BattlenetIds
        {
            get => battlenetIds ?? Enumerable.Empty<int>();
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
                Process.Start(uri);
            }
        }

        public AnalysisViewModel(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task LoadAsync()
        {
            GetSession.Response sessionResponse = await mediator.Send(new GetSession.Query());
            GetUserSettings.Response settingsResponse = await mediator.Send(new GetUserSettings.Query());
            GetKnownBattleNetIds.Response battleNetResponse = await mediator.Send(new GetKnownBattleNetIds.Query());

            Session = sessionResponse.Session;
            UserSettings = settingsResponse.UserSettings;
            BattlenetIds = battleNetResponse.BattleNetIds;
        }
    }
}
