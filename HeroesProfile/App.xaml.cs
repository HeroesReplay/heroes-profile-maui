using HeroesProfile.UI.Services.Tray;
using Microsoft.Extensions.Logging;

namespace HeroesProfile.UI;

public partial class App : Application
{
    private ILogger<App> Logger { get; }
    
    public App()
    {
        InitializeComponent();
        MainPage = new MainPage();
        Logger = IPlatformApplication.Current!.Services.GetRequiredService<ILogger<App>>();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);
        window.Title = "HeroesProfile Uploader";
        
        window.Stopped += (sender, args) =>
        {
            
            Logger.LogInformation("Window stopped");
        };
        
        window.Backgrounding += (sender, args) =>
        {
            Logger.LogInformation("Window backgrounding");
        };
        
        window.Activated += (sender, args) =>
        {
            Logger.LogInformation("Window activated");
        };
        
        return window;
    }

    protected override void OnStart()
    {
        IPlatformTrayService tray = IPlatformApplication.Current!.Services.GetRequiredService<IPlatformTrayService>();
        tray.Initialize();
        base.OnStart();
    }

    protected override void OnSleep()
    {
        base.OnSleep();
    }
    protected override void OnResume()
    {
        base.OnResume();
    }

    public override void CloseWindow(Window window)
    {
        base.CloseWindow(window);
    }

    protected override void CleanUp()
    {
        base.CleanUp();
    }
}