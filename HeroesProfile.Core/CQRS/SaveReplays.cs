using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.Models;
using HeroesProfile.Core.Services.Repositories;

using MediatR;

namespace HeroesProfile.Core.CQRS
{
    public static class SaveReplays
    {
        public class Request : IRequest<Response>
        {
            public IEnumerable<ReplayParseData> ParseDatas { get; init; }

            public Request(IEnumerable<ReplayParseData> parseDatas)
            {
                ParseDatas = parseDatas;
            }

            public Request(params ReplayParseData[] parseDatas) : this(parseDatas.AsEnumerable())
            {

            }
        }

        public record Response(List<StoredReplay> StoredReplay);

        public class Handler : IRequestHandler<Request, Response>
        {
            private ReplaysRepository replaysRepository;

            public Handler(ReplaysRepository replaysRepository)
            {
                this.replaysRepository = replaysRepository;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
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