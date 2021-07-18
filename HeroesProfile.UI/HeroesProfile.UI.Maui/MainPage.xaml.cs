using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace HeroesProfile.UI.Maui
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage, IPage
    {
        static bool isSetup = false;

        public MainPage()
        {
            InitializeComponent();

            if (!isSetup)
            {
                isSetup = true;

                SetupTrayIcon();
            }
        }

        private void SetupTrayIcon()
        {
            var trayService = ServiceProvider.GetService<ITrayService>();

            if (trayService != null)
            {
                trayService.Initialize();
                trayService.ClickHandler = () =>
                {
                    var notificationService = ServiceProvider.GetService<INotificationService>();

                    notificationService?.ShowNotification("Heroes Profile Desktop", "Test Notification");
                };
            }
        }
    }
}
