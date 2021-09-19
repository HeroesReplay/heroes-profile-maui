using System;

namespace HeroesProfile.Maui
{
    public interface ITrayService
    {
        void Initialize();

        Action ClickHandler { get; set; }
    }
}