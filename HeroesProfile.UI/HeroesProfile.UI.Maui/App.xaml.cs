using System.Reflection;

using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace HeroesProfile.UI.Maui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            return new Window(new MainPage()) { Title = "Heroes Profile Desktop - " + Assembly.GetExecutingAssembly().GetName().Version.ToString() };
        }
    }
}
