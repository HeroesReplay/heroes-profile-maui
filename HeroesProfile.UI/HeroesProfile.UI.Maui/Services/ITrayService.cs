using System;

namespace HeroesProfile.UI.Maui.Services
{
    public interface ITrayService
    {
        void Initialize();

        Action ClickHandler { get; set; }
    }
}