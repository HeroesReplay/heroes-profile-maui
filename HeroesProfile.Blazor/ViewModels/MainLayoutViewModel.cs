using Blazorise.Localization;

using ReactiveUI;
using System.Diagnostics;

namespace HeroesProfile.Blazor.ViewModels;

public class MainLayoutViewModel(ITextLocalizerService LocalizationService) : ReactiveObject
{
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
            Process.Start("open", uri);
        }
    }

    protected ITextLocalizerService LocalizationService { get; set; } = LocalizationService;
}
