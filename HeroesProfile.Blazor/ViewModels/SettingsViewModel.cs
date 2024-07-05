using Blazorise;
using HeroesProfile.Core.CQRS.Commands.UserSettings;
using HeroesProfile.Core.CQRS.Queries;
using MediatR;
using ReactiveUI;

namespace HeroesProfile.Blazor.ViewModels;

public class SettingsViewModel(IMediator mediator, INotificationService notifications) : ReactiveObject
{
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


    public async Task LoadAsync()
    {
        GetUserSettings.Response response = await mediator.Send(new GetUserSettings.Query());

        Form = new UserSettingsForm()
        {
            EnablePostMatch = response.UserSettings.EnablePostMatch,
            EnablePreMatch = response.UserSettings.EnablePreMatch,
        };
    }

    public async Task SaveAsync()
    {
        if (await Validator.ValidateAll())
        {
            await mediator.Send(new UpdateUserSettings.Command(new Core.Models.UserSettings()
            {
                EnablePostMatch = Form.EnablePostMatch,
                EnablePreMatch = Form.EnablePreMatch,
            }));

            await notifications.Success("Settings saved.");
        }
        else
        {
            await notifications.Error("Validation errors.");
        }
    }


    public class UserSettingsForm
    {
        public bool EnablePostMatch { get; set; }
        public bool EnablePreMatch { get; set; }
    }
}