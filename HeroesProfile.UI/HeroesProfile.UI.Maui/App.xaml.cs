using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;

namespace HeroesProfile.UI.Maui
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        private Window window;
        private MainPage mainPage;

        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            On<Microsoft.Maui.Controls.PlatformConfiguration.Windows>().SetImageDirectory("Assets");

            mainPage = new MainPage();
            window = new Window(mainPage) { Title = "Heroes Profile Desktop - Alpha" };
            
            return window;
        }        
    }
}
