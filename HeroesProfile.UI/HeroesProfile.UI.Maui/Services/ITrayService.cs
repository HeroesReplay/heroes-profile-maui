using System;

namespace HeroesProfile.UI.Maui
{
    public interface ITrayService
    {
        void Initialize();

        Action ClickHandler { get; set; }
    }
}