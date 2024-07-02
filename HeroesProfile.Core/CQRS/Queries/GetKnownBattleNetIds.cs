using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Models;
using MediatR;

namespace HeroesProfile.Core.CQRS.Queries;

public static class GetKnownBattleNetIds
{
    public record Query : IRequest<Response>;

    public record Response(IEnumerable<long> BattleNetIds);


    public class Handler : IRequestHandler<Query, Response>
    {
        private readonly AppSettings appSettings;

        public Handler(AppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        public Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            IEnumerable<long> battleNetIds = new DirectoryInfo(appSettings.GameDocumentsDirectory)
                    .EnumerateDirectories("*-*", SearchOption.AllDirectories)
                    .Where(directory => long.TryParse(directory.Parent.Name, out var accountId))
                    .Select(directory => long.Parse(directory.Name.Split("-").Last()))
                    .Distinct();

            return Task.FromResult(new Response(battleNetIds.ToList()));
        }
    }
}