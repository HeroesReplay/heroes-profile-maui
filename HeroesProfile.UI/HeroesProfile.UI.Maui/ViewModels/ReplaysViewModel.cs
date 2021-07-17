using Blazorise;

using HeroesProfile.Core.CQRS.Notifications;
using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;

using MediatR;

using Microsoft.AspNetCore.Components;

using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HeroesProfile.UI.Maui.ViewModels
{
    public class ReplaysViewModel : ReactiveObject
    {
        private readonly IMediator mediator;

        public ReplaysViewModel(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task LoadAsync(CancellationToken cancellationToken)
        {
            GetReplays.Response result = await this.mediator.Send(new GetReplays.Query(new List<GetReplays.Filter>()), cancellationToken);
            StoredReplays = result.Replays.Select(storedReplay => new GridItem(storedReplay)).OrderByDescending(x => x.Updated).ThenByDescending(x => x.Created);
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

            public GridItem(StoredReplay replay)
            {
                Item = replay;
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

            // Customize the format 

            public DateTime Created => Item.Created;
            public DateTime Updated => Item.Updated;
            public string Path => System.IO.Path.GetFileName(Item.Path);
            public bool Deleted => !System.IO.File.Exists(Item.Path);
            public string Fingerprint => Item.Fingerprint;
            public ParseResult ParseResult => Item.ParseResult;
            public UploadStatus UploadStatus => Item.UploadStatus;
            public ProcessStatus ProcessStatus => Item.ProcessStatus;
        }
    }
}