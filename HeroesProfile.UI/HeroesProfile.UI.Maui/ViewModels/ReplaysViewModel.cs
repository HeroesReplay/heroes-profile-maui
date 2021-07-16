using HeroesProfile.Core.CQRS.Notifications;
using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;

using MediatR;

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
            return result.Replays.OrderByDescending(x => x.Updated).ThenByDescending(x => x.Created).Select(storedReplay => new GridItem(storedReplay));
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
