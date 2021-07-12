using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using Heroes.ReplayParser;

using MediatR;

using Microsoft.Extensions.Logging;

namespace HeroesProfile.Core.Services.PreProcessors
{
    public class FileInfoConverter : JsonConverter<FileInfo>
    {
        public override FileInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, FileInfo value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.FullName);
        }
    }

    public class ByteArrayConverter : JsonConverter<byte[]>
    {
        public const long OneKB = 1024;

        public const long OneMB = OneKB * OneKB;

        public const long OneGB = OneMB * OneKB;

        public const long OneTB = OneGB * OneKB;

        public static string BytesToHumanReadable(long bytes)
        {
            return bytes switch
            {
                (< OneKB) => $"{bytes}B",
                (>= OneKB) and (< OneMB) => $"{bytes / OneKB}KB",
                (>= OneMB) and (< OneGB) => $"{bytes / OneMB}MB",
                (>= OneGB) and (< OneTB) => $"{bytes / OneMB}GB",
                (>= OneTB) => $"{bytes / OneTB}"
                //...
            };
        }

        public override byte[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value != null ? BytesToHumanReadable(value.LongLength) : "null");
        }
    }

    public class ReplayConverter : JsonConverter<Replay>
    {
        public override Replay? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Replay replay, JsonSerializerOptions options)
        {
            writer.WriteStringValue(replay != null ? replay.RandomValue.ToString() : "null");
        }
    }

    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TRequest> logger;

        private JsonSerializerOptions options = new JsonSerializerOptions()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            Converters =
            {
                new FileInfoConverter(),
                new ReplayConverter(),
                new ByteArrayConverter()
            }
        };

        public LoggingBehavior(ILogger<TRequest> logger)
        {
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestName = request.GetType().Name;
            var requestGuid = Guid.NewGuid().ToString();

            var requestNameWithGuid = $"{requestName} [{requestGuid}]";

            logger.LogInformation($"[START] {requestNameWithGuid}");
            TResponse response;

            try
            {
                try
                {
                    logger.LogInformation($"[PROPS] {requestNameWithGuid} {JsonSerializer.Serialize(request, options)}");
                }
                catch (NotSupportedException e)
                {
                    logger.LogInformation($"[Serialization ERROR] {requestNameWithGuid} Could not serialize the request.");
                }

                response = await next();
            }
            finally
            {
                stopwatch.Stop();
                logger.LogInformation($"[END] {requestNameWithGuid}; Execution time={stopwatch.ElapsedMilliseconds}ms");
            }

            return response;
        }
    }
}
