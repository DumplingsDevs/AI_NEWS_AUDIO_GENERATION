using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using AiNews.Dtos;
using AiNews.Extensions;
using AiNews.OpenAI;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AiNews;

public class GenerateAudio
{
    private readonly IOpenAiClient _openAiClient;
    private readonly ILogger<GenerateAudio> _logger;

    public GenerateAudio(IOpenAiClient openAiClient, ILogger<GenerateAudio> logger)
    {
        _openAiClient = openAiClient;
        _logger = logger;
    }

    [Function("GenerateAudio")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        [FromBody] AudioGenerateDto dto)
    {
        var responseBytes = await GetAudio(dto.Input, dto.Separator);

        return await req.GetFileResponseAsync(responseBytes);
    }

    private async Task<byte[]> GetAudio(string input, string separator)
    {
        var articles = ContentAggregator.GetContentsForAudio(input, separator, 4095).AsParallel().AsOrdered();
        var audioResults = new ConcurrentBag<byte[]>();

        var options = new ParallelOptions() { MaxDegreeOfParallelism = 3 };
        await Parallel.ForEachAsync(articles, options, async (article, cancellationToken) =>
        {
            var audio = await _openAiClient.GetAudio(article);
            audioResults.Add(audio);
        });

        return audioResults.SelectMany(x => x).ToArray();
    }
}