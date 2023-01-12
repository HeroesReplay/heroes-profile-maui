namespace HeroesProfile.UI.Services;

public interface ITrayService
{
    void Initialize();

    Action ClickHandler { get; set; }
}