using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;
using MediatR;


namespace HeroesProfile.Core.CQRS.Commands
{
    public static class SaveReplays
    {
        public record Command(params ReplayParseData[] ParseDatas): IRequest<Response>;

        public record Response(List<StoredReplay> StoredReplays);

        public class Handler : IRequestHandler<Command, Response>
        {
            private ReplaysRepository replaysRepository;

            public Handler(ReplaysRepository replaysRepository)
            {
                this.replaysRepository = replaysRepository;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var storedReplays = request.ParseDatas.Select(data => new StoredReplay()
                {
                    Created = data.File.CreationTime,
                    Path = data.File.FullName,
                    ParseResult = data.ParseResult,
                    Fingerprint = data.Fingerprint,
                    ProcessStatus = ProcessStatus.Pending
                });

                var list = storedReplays.ToList();

                await replaysRepository.InsertAsync(list, cancellationToken);

                return new Response(list);
            }
        }
    }


}