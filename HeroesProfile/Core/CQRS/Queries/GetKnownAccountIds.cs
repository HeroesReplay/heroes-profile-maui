using HeroesProfile.UI.Core.Models;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Queries;

public static class GetKnownAccountIds
{
    public record Query : IRequest<Response>;

    public record Response(IEnumerable<long> AccountIds);

    public class Handler(AppSettings appSettings) : IRequestHandler<Query, Response>
    {
        public Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            IEnumerable<long> accountIds = new DirectoryInfo(appSettings.GameDocumentsDirectory)
                .EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
                .Where(directory => long.TryParse(directory.Parent.Name, out var accountId))
                .Select(directory => long.Parse(directory.Parent.Name))
                .Distinct();

            return Task.FromResult(new Response(accountIds.ToList()));
        }
    }

}