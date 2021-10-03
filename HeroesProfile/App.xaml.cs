
using System.Reflection;

using MauiApp2.Services;

using Microsoft.Maui;

using Application = Microsoft.Maui.Controls.Application;

namespace MauiApp2
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new MainPage();
        }

        protected override Microsoft.Maui.Controls.Window CreateWindow(IActivationState activationState)
        {
            var tray = ServiceProvider.GetService<ITrayService>();
            tray.Initialize();


            var window = base.CreateWindow(activationState);
            window.Title = "Heroes Profile - Alpha";
            return window;
        }
    }
}
