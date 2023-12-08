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
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req, [FromBody] AudioGenerateDto dto)
    {
        var audioResults = await Task.WhenAll(ContentAggregator.GetContentsForAudio(dto.Input, dto.Separator, 4095).Select(
            async x => await _openAiClient.GetAudio(x)));

        var responseBytes = audioResults.SelectMany(x => x).ToArray();
        return await req.GetFileResponseAsync(responseBytes);
    }
}