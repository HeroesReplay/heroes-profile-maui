using System.Diagnostics;
using HeroesProfile.UI.Core.CQRS.Queries;
using HeroesProfile.UI.Core.Models;
using MediatR;
using ReactiveUI;

namespace HeroesProfile.UI.ViewModels;


public class AnalysisViewModel(IMediator mediator) : ReactiveObject
{
    public bool HasBattleLobby => Session?.BattleLobby != null;
    public bool HasStormSave => Session?.StormSave != null;
    public bool HasStormReplay => Session?.StormReplay != null;
    public bool HasPreMatch => session?.PreMatchUri != null;
    public bool HasPostMatch => session?.PostMatchUri != null;

    private SessionData? session;
    private UserSettings? settings;
    private IEnumerable<long> battlenetIds = [];

    public IEnumerable<long> BattlenetIds
    {
        get => battlenetIds;
        set
        {
            battlenetIds = value;
            this.RaisePropertyChanged();
        }
    }
    
    public IEnumerable<long> AccountIds
    {
        get => battlenetIds;
        set
        {
            battlenetIds = value;
            this.RaisePropertyChanged();
        }
    }

    public SessionData? Session
    {
        get => session;
        set
        {
            session = value;
            this.RaisePropertyChanged();
        }
    }

    public UserSettings? UserSettings
    {
        get => settings;
        set
        {
            settings = value;
            this.RaisePropertyChanged();
        }
    }

    //public bool IsPostMatchEnabled => UserSettings.EnablePostMatch;
    //public bool IsPreMatchEnabled => UserSettings.EnablePreMatch;


    public void OpenInBrowser(string uri)
    {
        if (OperatingSystem.IsWindows())
        {
            Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
        }
        else if (OperatingSystem.IsMacCatalyst())
        {
            Process.Start("open", uri);
        }
    }

    public async Task LoadAsync()
    {
        GetUserSettings.Response settingsResponse = await mediator.Send(new GetUserSettings.Query());
        UserSettings = settingsResponse.UserSettings;

        GetSession.Response sessionResponse = await mediator.Send(new GetSession.Query());
        Session = sessionResponse.Session;

        GetKnownBattleNetIds.Response battleNetResponse = await mediator.Send(new GetKnownBattleNetIds.Query());
        BattlenetIds = battleNetResponse.BattleNetIds;
        
        GetKnownAccountIds.Response accountIdsResponse = await mediator.Send(new GetKnownAccountIds.Query());
    }
}
