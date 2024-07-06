using HeroesProfile.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace HeroesProfile.UI;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new MainPage();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var tray = HeroesProfile.UI.Services.ServiceProvider.Current.Services.GetRequiredService<ITrayService>();
        tray.Initialize();
        
        var window = base.CreateWindow(activationState);
        window.Title = "Heroes Profile - Alpha";
        return window;
    }
}