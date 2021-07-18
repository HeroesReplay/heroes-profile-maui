using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;

using MediatR;

using ReactiveUI;

using System.Threading.Tasks;

namespace HeroesProfile.UI.Maui.ViewModels
{
    public class AnalysisViewModel : ReactiveObject
    {
        private readonly IMediator mediator;

        public bool HasBattleLobby => Session.BattleLobby != null;
        public bool HasStormSave => Session.StormSave != null;
        public bool HasStormReplay => Session.StormReplay != null;

        public bool HasPreMatch => false;
        public bool HasPostMatch => false;


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
