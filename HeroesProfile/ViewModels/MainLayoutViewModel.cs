using Blazorise.Localization;

using ReactiveUI;

using System;
using System.Diagnostics;

namespace MauiApp2.ViewModels
{
    public class MainLayoutViewModel : ReactiveObject
    {
        public MainLayoutViewModel(ITextLocalizerService LocalizationService)
        {
            this.LocalizationService = LocalizationService;
        }

        public void OpenInBrowser(string uri)
        {
            if (OperatingSystem.IsWindows())
            {
                using (Process proc = new Process())
                {
                    proc.StartInfo.UseShellExecute = true;
                    proc.StartInfo.FileName = uri;
                    proc.Start();
                }
            }
            else if (OperatingSystem.IsMacCatalyst())
            {
                Process.Start(uri);
            }
        }

        protected ITextLocalizerService LocalizationService { get; set; }
    }
}
