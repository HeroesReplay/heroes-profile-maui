using System.Reflection;

using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;

using Application = Microsoft.Maui.Controls.Application;

namespace HeroesProfile.Maui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            return new Window(MainPage) { Title = "Heroes Profile Desktop - " + Assembly.GetExecutingAssembly().GetName().Version.ToString() };
        }
    }
}
