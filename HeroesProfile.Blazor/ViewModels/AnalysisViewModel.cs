using MediatR;

using ReactiveUI;
using System.Diagnostics;
using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;

namespace HeroesProfile.Blazor.ViewModels;


public class AnalysisViewModel(IMediator mediator, IEnumerable<long> battlenetIds) : ReactiveObject
{
    public bool HasBattleLobby => Session?.BattleLobby != null;
    public bool HasStormSave => Session?.StormSave != null;
    public bool HasStormReplay => Session?.StormReplay != null;
    public bool HasPreMatch => session?.PreMatchUri != null;
    public bool HasPostMatch => session?.PostMatchUri != null;

    private SessionData session;
    private UserSettings settings;

    public IEnumerable<long> BattlenetIds
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

    public bool IsPostMatchEnabled => UserSettings.EnablePostMatch;
    public bool IsPreMatchEnabled => UserSettings.EnablePreMatch;


    public void OpenInBrowser(string uri)
    {
        if (OperatingSystem.IsWindows())
        {
            using (Process proc = new Process())
            {
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.FileName = uri;
                proc.Start();
            }
        }
        else if (OperatingSystem.IsMacCatalyst())
        {
            Process.Start("open", uri);
        }
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
