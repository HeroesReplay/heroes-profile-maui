using Blazorise;

using MauiApp2.Core.Clients;
using MauiApp2.Core.CQRS.Commands.UserSettings;
using MauiApp2.Core.CQRS.Queries;

using MediatR;

using ReactiveUI;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MauiApp2.ViewModels
{
    public class SettingsViewModel : ReactiveObject
    {
        private readonly IMediator mediator;
        private readonly TalentsClient talentsClient;
        private readonly INotificationService notifications;
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

        public Validations Validator { get; set; }


        public SettingsViewModel(IMediator mediator, TalentsClient talentsClient, INotificationService notifications)
        {
            this.mediator = mediator;
            this.talentsClient = talentsClient;
            this.notifications = notifications;
        }

        public async Task LoadAsync()
        {
            GetUserSettings.Response response = await mediator.Send(new GetUserSettings.Query());

            Form = new UserSettingsForm()
            {
                EnablePostMatch = response.UserSettings.EnablePostMatch,
                EnablePredictions = response.UserSettings.EnablePredictions,
                EnablePreMatch = response.UserSettings.EnablePreMatch,
                EnableTwitchExtension = response.UserSettings.EnableTalentsExtension,
                EnableDiscordEnhancement = response.UserSettings.EnableDiscordEnhancement,
                EnableDiscordPreMatch = response.UserSettings.EnableDiscordPreMatch,

                // Needed for twitch features
                BroadcasterId = response.UserSettings.BroadcasterId,
                HeroesProfileApiEmail = response.UserSettings.HeroesProfileApiEmail,
                HeroesProfileTwitchKey = response.UserSettings.HeroesProfileTwitchKey,
                HeroesProfileUserId = response.UserSettings.HeroesProfileUserId
            };
        }

        public async Task SaveAsync()
        {
            if (await Validator.ValidateAll())
            {
                await mediator.Send(new UpdateUserSettings.Command(new Core.Models.UserSettings()
                {
                    BroadcasterId = Form.BroadcasterId,
                    EnablePostMatch = Form.EnablePostMatch,
                    EnablePredictions = Form.EnablePredictions,
                    EnablePreMatch = Form.EnablePreMatch,
                    EnableTalentsExtension = Form.EnableTwitchExtension,
                    EnableDiscordEnhancement = Form.EnableDiscordEnhancement,
                    EnableDiscordPreMatch = Form.EnableDiscordPreMatch,
                    HeroesProfileApiEmail = Form.HeroesProfileApiEmail,
                    HeroesProfileTwitchKey = Form.HeroesProfileTwitchKey,
                    HeroesProfileUserId = Form.HeroesProfileUserId
                }));

                await notifications.Success("Settings saved.");
            }
            else
            {
                await notifications.Error("Validation errors.");
            }
        }

        //private string[] ParseEntries(string input)
        //{
        //    try
        //    {
        //        return input.Split(new[] { " ", Environment.NewLine, "," }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        //    }
        //    catch
        //    {
        //        return Array.Empty<string>();
        //    }
        //}

        //public void ValidateBattleTags(ValidatorEventArgs e)
        //{
        //    var input = Convert.ToString(e.Value);

        //    if (Form.EnablePredictions && string.IsNullOrWhiteSpace(input))
        //    {
        //        e.Status = ValidationStatus.Error;
        //        e.ErrorText = "Provide your battletag account(s) for auto predictions.";
        //    }
        //    else if (!string.IsNullOrWhiteSpace(input))
        //    {
        //        try
        //        {
        //            var parsed = ParseEntries(input).All(x => int.TryParse(x.Split('#')[1], out var tag) && !char.IsDigit(x.Split('#')[0][0]));

        //            if (parsed)
        //            {
        //                e.Status = ValidationStatus.Success;
        //            }
        //            else
        //            {
        //                e.Status = ValidationStatus.Error;
        //                e.ErrorText = "Entries should be seperated by a comma (,) or new line";
        //            }
        //        }
        //        catch
        //        {
        //            e.Status = ValidationStatus.Error;
        //        }
        //    }
        //    else
        //    {
        //        e.Status = ValidationStatus.Success;
        //    }
        //}

        public void ValidateHeroesEmail(ValidatorEventArgs e)
        {
            if (Form.EnableTwitchExtension)
            {
                var input = Convert.ToString(e.Value);

                if (string.IsNullOrWhiteSpace(input) || !ValidationRule.IsEmail(input))
                {
                    e.Status = ValidationStatus.Error;
                    e.ErrorText = "Authentication required";
                }
            }
        }

        public async Task ValidateHeroesTwitchKey(ValidatorEventArgs e, CancellationToken token)
        {
            var input = Convert.ToString(e.Value);

            if (!Form.EnableTwitchExtension)
            {
                e.Status = ValidationStatus.None;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Form.HeroesProfileApiEmail) || string.IsNullOrWhiteSpace(input))
                {
                    e.Status = ValidationStatus.Error;
                    e.ErrorText = "Authentication required";
                }
                else
                {
                    var response = await talentsClient.GetUserIdByAuth(Form.HeroesProfileApiEmail, Form.BroadcasterId, input, token);

                    if (string.IsNullOrWhiteSpace(response.Error))
                    {
                        Form.HeroesProfileUserId = response.UserId;
                        e.Status = ValidationStatus.Success;
                    }
                    else
                    {
                        e.ErrorText = response.Error;
                        e.Status = ValidationStatus.Error;
                    }
                }
            }
        }

        //public void ValidateTwitchAccessToken(ValidatorEventArgs e)
        //{
        //    var input = Convert.ToString(e.Value);

        //    if (!Form.EnablePredictions)
        //    {
        //        e.Status = ValidationStatus.None;
        //    }
        //    else
        //    {
        //        e.Status = string.IsNullOrWhiteSpace(input) ? ValidationStatus.Error : ValidationStatus.Success;
        //        e.ErrorText = "Required for Twitch Predictions";
        //    }

        //}

        //public void ValidateTwitchClientId(ValidatorEventArgs e)
        //{
        //    var input = Convert.ToString(e.Value);

        //    if (!Form.EnablePredictions)
        //    {
        //        e.Status = ValidationStatus.None;
        //    }
        //    else
        //    {
        //        e.Status = string.IsNullOrWhiteSpace(input) ? ValidationStatus.Error : ValidationStatus.Success;
        //        e.ErrorText = "Required for Twitch Predictions";
        //    }
        //}

        public void ValidateBroadcasterId(ValidatorEventArgs e)
        {
            var input = Convert.ToString(e.Value);

            if (Form.EnablePredictions || Form.EnableTwitchExtension)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    e.Status = ValidationStatus.Error;
                    e.ErrorText = "Required for Twitch features";
                }
                else
                {
                    e.Status = ValidationStatus.None;
                }
            }
        }


        public void ValidateEnableTwitch(ValidatorEventArgs e)
        {
            var @checked = Convert.ToBoolean(e.Value);

            if (@checked)
            {
                if (string.IsNullOrWhiteSpace(Form.BroadcasterId) ||
                    string.IsNullOrWhiteSpace(Form.HeroesProfileUserId) ||
                    string.IsNullOrWhiteSpace(Form.HeroesProfileTwitchKey))
                {
                    e.Status = ValidationStatus.Error;
                    e.ErrorText = "Authentication required";
                }
            }
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
            public bool EnableDiscordPreMatch { get; internal set; }
            public bool EnableDiscordEnhancement { get; internal set; }
        }
    }
}
