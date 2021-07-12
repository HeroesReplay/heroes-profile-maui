using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.Models;
using HeroesProfile.Core.Services;

using MediatR;

namespace HeroesProfile.Core.CQRS
{
    public static class ProcessReplays
    {
        public record ProcessedReplay(StoredReplay StoredReplay, ReplayParseData ParseData);

        public record Response(IEnumerable<ProcessedReplay> Processed);

        public record Request : IRequest<Response>;

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly StormReplayScanner scanner;
            private readonly IMediator mediator;

            public Handler(StormReplayScanner scanner, IMediator mediator)
            {
                this.scanner = scanner;
                this.mediator = mediator;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                IEnumerable<FileInfo> newReplays = await scanner.GetNewReplays(cancellationToken);
                List<ProcessedReplay> processed = new List<ProcessedReplay>();

                foreach (FileInfo[] group in newReplays.Chunk(Math.Max(Environment.ProcessorCount / 4, 1)).ToList())
                {
                    // Parse batch in parallel
                    GetParsedReplay.Response[] parsedResponses = await Task.WhenAll(group.AsParallel().WithExecutionMode(ParallelExecutionMode.Default).WithCancellation(cancellationToken).Select(info => mediator.Send(new GetParsedReplay.Request(info), cancellationToken)).ToList());

                    // Save in serial for batch
                    SaveReplays.Response saveResponse = await mediator.Send(new SaveReplays.Request(parsedResponses.Select(x => x.Data)), cancellationToken);

                    // Map stored replays to parsed replays
                    List<ProcessedReplay> batch = parsedResponses.Select(response => new ProcessedReplay(saveResponse.StoredReplay.Find(x => string.Equals(x.Fingerprint, response.Data.Fingerprint, StringComparison.OrdinalIgnoreCase)), response.Data)).ToList();

                    processed.AddRange(batch);
                }

                return new Response(processed);
            }
        }
    }
}