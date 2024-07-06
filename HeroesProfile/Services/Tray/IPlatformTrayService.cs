namespace HeroesProfile.UI.Services.Tray;

public interface IPlatformTrayService
{
    void Initialize();

    Action ClickHandler { get; set; }
}