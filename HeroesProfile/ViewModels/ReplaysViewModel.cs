using System.Diagnostics;
using Heroes.StormReplayParser;
using HeroesProfile.UI.Core.CQRS.Queries;
using HeroesProfile.UI.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Components.Web;
using ReactiveUI;
using Color = Blazorise.Color;

namespace HeroesProfile.UI.ViewModels;

public class ReplaysViewModel(IMediator mediator, AppSettings appSettings) : ReactiveObject
{
    public static readonly Uri RelativeMatchUri = new Uri("Match/Single", UriKind.Relative);

    private readonly Uri matchUri = new(appSettings.HeroesProfileUri, RelativeMatchUri);

    public async Task LoadAsync(CancellationToken cancellationToken)
    {
        GetReplays.Response result = await mediator.Send(new GetReplays.Query(), cancellationToken);
        StoredReplays = result.Replays.Select(r => new GridItem(r, matchUri)).ToList();
    }

    private List<GridItem> storedReplays;

    public List<GridItem> StoredReplays
    {
        get => storedReplays;
        set
        {
            storedReplays = value;
            this.RaisePropertyChanged();
        }
    }

    public GridItem SelectedRow { get; set; }

    public class GridItem
    {
        public StoredReplay Item { get; }

        public GridItem(StoredReplay replay, Uri matchUri)
        {
            Item = replay;
            WebLink = replay.ReplayId != null ? new Uri(matchUri, $"?replayID={Item.ReplayId.Value}") : null;
        }

        public Color ParseStatusColor
        {
            get
            {
                return ParseStatus switch
                {
                    StormReplayParseStatus.Success => Color.Success,
                    StormReplayParseStatus.PTRRegion => Color.Info,
                    StormReplayParseStatus.PreAlphaWipe => Color.Info,
                    StormReplayParseStatus.Incomplete => Color.Danger,
                    StormReplayParseStatus.TryMeMode => Color.Danger,
                    StormReplayParseStatus.Exception => Color.Danger,
                    StormReplayParseStatus.FileNotFound => Color.Danger,
                    StormReplayParseStatus.FileSizeTooLarge => Color.Danger,
                    StormReplayParseStatus.UnexpectedResult => Color.Danger,
                    StormReplayParseStatus.Unknonwn => Color.Danger
                };
            }
        }

        public Color ProcessStatusColor
        {
            get
            {
                return ProcessStatus switch
                {
                    ProcessStatus.Pending => Color.Info,
                    ProcessStatus.Success => Color.Success,
                    ProcessStatus.Duplicate => Color.Warning,
                    ProcessStatus.NotSupported => Color.Success,
                    ProcessStatus.Error => Color.Danger,
                    _ => Color.Warning,
                };
            }
        }


        public void OpenInBrowser(MouseEventArgs e)
        {
            if (WebLink != null)
            {
                if (OperatingSystem.IsWindows())
                {
                    using (Process proc = new Process())
                    {
                        proc.StartInfo.UseShellExecute = true;
                        proc.StartInfo.FileName = WebLink.ToString();
                        proc.Start();
                    }
                }
                else if (OperatingSystem.IsMacCatalyst())
                {
                    Process.Start("open", WebLink.ToString());
                }
            }
        }

        public void LaunchReplay(MouseEventArgs e)
        {
            if (OperatingSystem.IsWindows())
            {
                using (Process proc = new Process())
                {
                    proc.StartInfo.UseShellExecute = true;
                    proc.StartInfo.FileName = Item.Path;
                    proc.Start();
                }
            }
            else if (OperatingSystem.IsMacCatalyst())
            {
                using (Process proc = new Process())
                {
                    proc.StartInfo.UseShellExecute = true;
                    proc.StartInfo.FileName = Item.Path;
                    proc.Start();
                }
            }
        }

        public Uri? WebLink { get; set; }
        public DateTime Created => Item.Created;
        public DateTime Updated => Item.Updated;
        public string Path => System.IO.Path.GetFileNameWithoutExtension(Item.Path);
        public bool Exists => File.Exists(Item.Path);
        public string? Fingerprint => Item.Fingerprint;
        public ProcessStatus ProcessStatus => Item.ProcessStatus;
        public StormReplayParseStatus ParseStatus => Item.ParseStatus ?? StormReplayParseStatus.Unknonwn;
    }
}