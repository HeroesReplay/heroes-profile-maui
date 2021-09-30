
using System.Reflection;

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
            var window = base.CreateWindow(activationState);
            
            window.Title = "Heroes Profile - Alpha";
            
            return window;
        }
    }
}
