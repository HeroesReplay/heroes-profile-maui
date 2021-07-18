using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;

using MediatR;

using ReactiveUI;

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

        public SessionData Session
        {
            get
            {
                return session;
            }
            set
            {
                session = value;
                this.RaisePropertyChanged();
            }
        }

        public void OpenInBrowser(string uri)
        {
            if (System.OperatingSystem.IsWindows())
            {
                Process.Start(uri);
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
            GetSession.Response response = await this.mediator.Send(new GetSession.Query());

            Session = response.Session;
        }
    }
}
