using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.Logging;

namespace HeroesProfile.Core.CQRS.Behaviours;


public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> logger;
    private readonly JsonSerializerOptions options;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger, IEnumerable<JsonConverter> converters)
    {
        this.logger = logger;

        options = new JsonSerializerOptions()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        foreach (var converter in converters)
        {
            options.Converters.Add(converter);
        }
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestName = request.GetType().DeclaringType?.Name ?? request.GetType().Name;
        var requestGuid = Guid.NewGuid().ToString();

        var requestNameWithGuid = $"{requestName} [{requestGuid}]";

        logger.LogInformation($"[START] {requestNameWithGuid}");
        TResponse? response;

        try
        {
            try
            {
                logger.LogInformation($"[PROPS] {requestNameWithGuid} {JsonSerializer.Serialize(request, options)}");
            }
            catch (NotSupportedException)
            {
                logger.LogInformation($"[Serialization ERROR] {requestNameWithGuid} Could not serialize the request.");
            }

            response = await next();

            try
            {
                logger.LogInformation($"[PROPS] [RESP] {requestNameWithGuid} {JsonSerializer.Serialize(response, options)}");
            }
            catch (NotSupportedException)
            {
                logger.LogInformation($"[Serialization ERROR] {requestNameWithGuid} Could not serialize the response.");
            }
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation($"[END] {requestNameWithGuid}; Execution time={stopwatch.ElapsedMilliseconds}ms");
        }

        return response;
    }
}
