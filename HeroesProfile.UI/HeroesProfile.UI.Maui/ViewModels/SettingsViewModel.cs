using Blazorise;

using HeroesProfile.Core.CQRS.Queries;

using MediatR;

using ReactiveUI;

using System;
using System.Linq;
using System.Threading.Tasks;

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
        }

        public async Task LoadAsync()
        {
            GetUserSettings.Response response = await mediator.Send(new GetUserSettings.Query());

            Form = new UserSettingsForm()
            {
                BattleTags = string.Join(Environment.NewLine, response.UserSettings.BattleTags),
                BroadcasterId = response.UserSettings.BroadcasterId,
                EnablePostMatch = response.UserSettings.EnablePostMatch,
                EnablePredictions = response.UserSettings.EnablePredictions,
                EnablePreMatch = response.UserSettings.EnablePreMatch,
                EnableTwitchExtension = response.UserSettings.EnableTwitchExtension,
                HeroesProfileApiEmail = response.UserSettings.HeroesProfileApiEmail,
                HeroesProfileTwitchKey = response.UserSettings.HeroesProfileTwitchKey,
                TwitchClientId = response.UserSettings.TwitchClientId,
                TwitchAccessToken = response.UserSettings.TwitchAccessToken,
                HeroesProfileUserId = response.UserSettings.HeroesProfileUserId
            };
        }

        public async Task SaveAsync()
        {
            await mediator.Send(new Core.CQRS.Commands.UpdateUserSettings.Command(new Core.Models.UserSettings()
            {
                BattleTags = Form.BattleTags.Split(new[] { Environment.NewLine, " " }, StringSplitOptions.TrimEntries).ToList(),
                BroadcasterId = Form.BroadcasterId,
                EnablePostMatch = Form.EnablePostMatch,
                EnablePredictions = Form.EnablePredictions,
                EnablePreMatch = Form.EnablePreMatch,
                EnableTwitchExtension = Form.EnableTwitchExtension,
                HeroesProfileApiEmail = Form.HeroesProfileApiEmail,
                HeroesProfileTwitchKey = Form.HeroesProfileTwitchKey,
                TwitchClientId = Form.TwitchClientId,
                TwitchAccessToken = Form.TwitchAccessToken,
                HeroesProfileUserId = Form.HeroesProfileUserId
            }));
        }

        public void ValidateBattleTags(ValidatorEventArgs e)
        {
            var input = Convert.ToString(e.Value);

            if (!string.IsNullOrWhiteSpace(input))
            {
                var entries = input.Split(new[] { " ", Environment.NewLine }, StringSplitOptions.TrimEntries);

                var valid = entries.All(entry =>
                {
                    try
                    {
                        var values = entry.Split('#');
                        return values.Length == 2 && int.TryParse(values[1], out var tag);
                    }
                    catch
                    {
                        return false;
                    }

                });

                e.Status = valid ? ValidationStatus.Success : ValidationStatus.Error;
            }
            else
            {
                e.Status = ValidationStatus.Success;
            }
        }


        public void ValidateUserId(ValidatorEventArgs e)
        {
            e.Status = (int.TryParse(Convert.ToString(e.Value), out int userId) ? ValidationStatus.Success : ValidationStatus.Error);
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
