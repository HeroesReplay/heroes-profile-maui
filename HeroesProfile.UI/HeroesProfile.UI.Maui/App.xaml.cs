using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;

namespace HeroesProfile.UI.Maui
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            Microsoft.Maui.Controls.Compatibility.Forms.Init(activationState);
            On<Microsoft.Maui.Controls.PlatformConfiguration.Windows>().SetImageDirectory("Assets");
            return new Window(new MainPage());
        }
    }
}
