using Blazorise;
using MediatR;
using ReactiveUI;
using System;

namespace HeroesProfile.UI.Maui.ViewModels
{
    public class SettingsViewModel : ReactiveObject
    {
        private readonly IMediator mediator;

        private UserSettingsForm form;

        public UserSettingsForm Form
        {
            get => form;
            set
            {
                form = value;
                this.RaisePropertyChanged();
            }
        }


        public SettingsViewModel(IMediator mediator)
        {
            this.mediator = mediator;

            Form = new UserSettingsForm();
        }

        public void SaveChanges()
        {
            
        }

        public void ValidateEmail(ValidatorEventArgs e)
        {
            var email = Convert.ToString(e.Value);

            e.Status = string.IsNullOrEmpty(email) ? ValidationStatus.None : email.Contains("@") ? ValidationStatus.Success : ValidationStatus.Error;
        }


        public class UserSettingsForm
        {
            public bool EnablePostMatch { get; set; }
            public bool EnablePreMatch { get; set; }
            public bool EnablePredictions { get; set; }
            public bool EnableTwitchExtension { get; set; }
            public string HeroesProfileTwitchKey { get; set; }
            public string HeroesProfileApiEmail { get; set; }
            public string BroadcasterId { get; set; }
            public string HeroesProfileUserId { get; set; }
            public string TwitchAccessToken { get; set; }
            public string TwitchClientId { get; set; }
            public string BattleTags { get; set; }
        }
    }
}
