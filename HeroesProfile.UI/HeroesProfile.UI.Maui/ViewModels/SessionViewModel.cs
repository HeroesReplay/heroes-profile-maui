using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;

using MediatR;

using ReactiveUI;

using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace HeroesProfile.UI.Maui.ViewModels
{
    public class SessionViewModel : ReactiveObject
    {
        private readonly IMediator mediator;


        private Session session;

        public string StormReplayJson
        {
            get
            {
                return Session.StormReplay != null ?
                JsonSerializer.Serialize(new
                {
                    Players = Session.StormReplay.Players.Select(x => $"{x.Name}"),
                    Length = Session.StormReplay.ReplayLength,
                    Messages = Session.StormReplay.Messages.Select(x => x.ChatMessage)
                }) : "";
            }
        }

        public string BattleLobbyJson
        {
            get
            {
                return Session.BattleLobby != null ? JsonSerializer.Serialize(Session.BattleLobby.Players.Select(x => $"{x.Name}"), new JsonSerializerOptions() { WriteIndented = true, IgnoreNullValues = true }) : "NULL";
            }
        }

        public string StormSaveJson
        {
            get
            {
                return Session.StormSave != null ? JsonSerializer.Serialize(Session.StormSave.Players.Select(x => new { Player = $"{x.Name}", Hero = x.HeroAttributeId, Talents = x.Talents }), new JsonSerializerOptions() { WriteIndented = true, IgnoreNullValues = true }) : "NULL";
            }
        }

        public string TwitchJson
        {
            get
            {
                return JsonSerializer.Serialize(new { Session.Prediction, Session.Extension });
            }
        }

        public Session Session
        {
            get
            {
                return session;
            }
            set
            {
                this.RaisePropertyChanged();
                session = value;
            }
        }

        public SessionViewModel(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task LoadSessionAsync()
        {
            var response = await this.mediator.Send(new GetSession.Query());

            Session = response.Session;
        }

        public ReactiveCommand<System.Reactive.Unit, Session> LoadSession { get; }
    }
}
