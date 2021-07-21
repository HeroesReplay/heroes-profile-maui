using System;

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

            SizeChanged += MainPage_SizeChanged;
        }

        private void MainPage_SizeChanged(object sender, System.EventArgs e)
        {
            BlazorWebView.HeightRequest = this.Height;
            BlazorWebView.WidthRequest = this.Width;
        }

        private void SetupTrayIcon()
        {
            var trayService = ServiceProvider.GetService<ITrayService>();

            if (trayService != null)
            {
                trayService.Initialize();
                trayService.ClickHandler = () =>
                {
                    var notificationService = ServiceProvider.GetService<IOSNotificationService>();

                    notificationService?.ShowNotification("Heroes Profile Desktop", "Test Notification");
                };
            }
        }
    }
}
