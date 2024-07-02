using HeroesProfile.UI.Services;

namespace HeroesProfile;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new MainPage();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var tray = HeroesProfile.UI.Services.ServiceProvider.GetService<ITrayService>();
        
        tray.Initialize();
        
        var window = base.CreateWindow(activationState);
        
        window.Title = "Heroes Profile - Alpha";

        return window;
    }
}