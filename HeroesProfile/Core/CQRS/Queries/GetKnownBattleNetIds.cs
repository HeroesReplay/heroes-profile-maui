using HeroesProfile.UI.Core.Models;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Queries;

public static class GetKnownBattleNetIds
{
    public record Query : IRequest<Response>;

    public record Response(IEnumerable<long> BattleNetIds);

    public class Handler(AppSettings appSettings) : IRequestHandler<Query, Response>
    {
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