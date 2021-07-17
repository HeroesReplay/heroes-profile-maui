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
using System.Threading.Tasks;

namespace HeroesProfile.UI.Maui.ViewModels
{
    public class ReplaysViewModel : ReactiveObject
    {
        private readonly IMediator mediator;
        private readonly ObservableAsPropertyHelper<IEnumerable<GridItem>> storedReplays;

        public ReplaysViewModel(IMediator mediator)
        {
            this.mediator = mediator;

            LoadStoredReplays = ReactiveCommand.CreateFromTask(LoadStoredReplaysAsync);
            this.storedReplays = LoadStoredReplays.ToProperty(this, x => x.StoredReplays, scheduler: RxApp.MainThreadScheduler);
        }

        private async Task<IEnumerable<GridItem>> LoadStoredReplaysAsync()
        {
            GetReplays.Response result = await this.mediator.Send(new GetReplays.Query(new List<GetReplays.Filter>()));
            return result.Replays.Select(storedReplay => new GridItem(storedReplay)).OrderByDescending(x => x.Updated).ThenByDescending(x => x.Created);
        }

        public ReactiveCommand<System.Reactive.Unit, IEnumerable<GridItem>> LoadStoredReplays { get; }

        public IEnumerable<GridItem> StoredReplays => storedReplays.Value;

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
                        ParseResult.Exception => Color.Danger,
                        ParseResult.FileNotFound => Color.Danger,
                        ParseResult.FileSizeTooLarge => Color.Danger,
                        ParseResult.PtrRegion => Color.Info,
                        ParseResult.PreAlphaWipe => Color.Info,
                        ParseResult.Incomplete => Color.Danger,
                        ParseResult.TryMeMode => Color.Danger,
                        ParseResult.UnexpectedResult => Color.Warning,
                        ParseResult.Success => Color.Success,
                        ParseResult.CustomGame => Color.Info,
                        _ => Color.Danger,
                    };
                }
            }

            public Color UploadStatusColor
            {
                get
                {
                    return UploadStatus switch
                    {
                        UploadStatus.None => Color.Info,
                        UploadStatus.InProgress => Color.Primary,

                        UploadStatus.Success => Color.Success,

                        UploadStatus.PtrRegion => Color.Info,
                        UploadStatus.Incomplete => Color.Info,
                        UploadStatus.TooOld => Color.Info,
                        UploadStatus.AiDetected => Color.Info,
                        UploadStatus.CustomGame => Color.Info,
                        UploadStatus.Duplicate => Color.Info,

                        UploadStatus.UploadError => Color.Danger,

                        _ => Color.Danger,
                    };
                }
            }

            public Color ProcessStatusColor
            {
                get
                {
                    return ProcessStatus switch
                    {
                        ProcessStatus.Success => Color.Success,

                        ProcessStatus.Duplicate => Color.Info,
                        ProcessStatus.NotSupported => Color.Info,
                        ProcessStatus.Pending => Color.Info,

                        ProcessStatus.Error => Color.Danger,

                        _ => Color.Danger,
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