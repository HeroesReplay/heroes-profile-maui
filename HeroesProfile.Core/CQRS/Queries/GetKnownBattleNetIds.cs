using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MauiApp2.Core.Models;

using MediatR;

namespace MauiApp2.Core.CQRS.Queries
{
    public static class GetKnownBattleNetIds
    {
        public record Query : IRequest<Response>;

        public record Response(IEnumerable<int> BattleNetIds);


        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly AppSettings appSettings;

            public Handler(AppSettings appSettings)
            {
                this.appSettings = appSettings;
            }

            public Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                IEnumerable<int> battleNetIds = new DirectoryInfo(appSettings.GameDocumentsDirectory)
                        .EnumerateDirectories("*-*", SearchOption.AllDirectories)
                        .Where(directory => int.TryParse(directory.Parent.Name, out var accountId))
                        .Select(directory => int.Parse(directory.Name.Split("-").Last()));

                return Task.FromResult(new Response(battleNetIds.ToList()));
            }
        }
    }
}