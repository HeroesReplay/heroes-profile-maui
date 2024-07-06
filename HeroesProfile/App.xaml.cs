using HeroesProfile.UI.Services.Tray;

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
        return base.CreateWindow(activationState);
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