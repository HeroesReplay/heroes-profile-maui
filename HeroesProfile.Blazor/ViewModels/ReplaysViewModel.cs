using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;

using MediatR;
using Microsoft.AspNetCore.Components.Web;

using ReactiveUI;
using System.Diagnostics;
using Color = Blazorise.Color;

namespace HeroesProfile.Blazor.ViewModels;

public class ReplaysViewModel : ReactiveObject
{
    private readonly IMediator mediator;

    public static Uri RelativeMatchUri = new Uri("Match/Single", UriKind.Relative);

    private Uri matchUri;

    public ReplaysViewModel(IMediator mediator, AppSettings appSettings)
    {
        this.mediator = mediator;
        matchUri = new Uri(appSettings.HeroesProfileUri, RelativeMatchUri);
    }

    public async Task LoadAsync(CancellationToken cancellationToken)
    {
        GetReplays.Response result = await mediator.Send(new GetReplays.Query(new List<GetReplays.Filter>()), cancellationToken);
        StoredReplays = result.Replays.Select(storedReplay => new GridItem(storedReplay, matchUri)).OrderByDescending(x => x.Updated).ThenByDescending(x => x.Created);
    }

    private IEnumerable<GridItem> storedReplays;

    public IEnumerable<GridItem> StoredReplays
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

        public GridItem(StoredReplay replay, Uri MatchUri)
        {
            Item = replay;
            WebLink = replay.ReplayId != null ? new Uri(MatchUri, $"?replayID={Item.ReplayId.Value}") : null;
        }

        public Color ParseResultColor
        {
            get
            {
                return ParseResult switch
                {
                    ParseResult.ComputerPlayerFound => Color.Info,
                    ParseResult.PtrRegion => Color.Info,
                    ParseResult.PreAlphaWipe => Color.Info,

                    ParseResult.Incomplete => Color.Danger,
                    ParseResult.TryMeMode => Color.Danger,
                    ParseResult.Exception => Color.Danger,
                    ParseResult.FileNotFound => Color.Danger,
                    ParseResult.FileSizeTooLarge => Color.Danger,

                    ParseResult.UnexpectedResult => Color.Warning,

                    ParseResult.Success => Color.Success,

                    ParseResult.CustomGame => Color.Info,

                    _ => Color.Warning,
                };
            }
        }

        public Color UploadStatusColor
        {
            get
            {
                return UploadStatus switch
                {
                    UploadStatus.Pending => Color.Info,
                    UploadStatus.PtrRegion => Color.Info,
                    UploadStatus.Incomplete => Color.Info,
                    UploadStatus.TooOld => Color.Info,
                    UploadStatus.AiDetected => Color.Info,
                    UploadStatus.CustomGame => Color.Info,
                    UploadStatus.Duplicate => Color.Info,
                    UploadStatus.Success => Color.Success,
                    UploadStatus.UploadError => Color.Danger,
                    _ => Color.Warning,
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
                    ProcessStatus.Duplicate => Color.Success,
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
                else if (OperatingSystem.IsMacOS())
                {
                    Process.Start("open", WebLink.ToString());
                }
            }
        }

        public void LaunchReplay(MouseEventArgs e)
        {
            if (WebLink != null)
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
                else if (OperatingSystem.IsMacOS())
                {
                    Process.Start("open", WebLink.ToString());
                }
            }
        }

        public Uri WebLink { get; set; }
        public DateTime Created => Item.Created;
        public DateTime Updated => Item.Updated;
        public string Path => System.IO.Path.GetFileName(Item.Path);
        public bool Exists => System.IO.File.Exists(Item.Path);
        public string Fingerprint => Item.Fingerprint;
        public ParseResult ParseResult => Item.ParseResult;
        public UploadStatus UploadStatus => Item.UploadStatus;
        public ProcessStatus ProcessStatus => Item.ProcessStatus;
    }
}