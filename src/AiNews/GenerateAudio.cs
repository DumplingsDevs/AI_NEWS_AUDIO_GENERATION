using AiNews.Dtos;
using AiNews.Exceptions;
using AiNews.Extensions;
using AiNews.OpenAI;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AiNews;

public class GenerateAudio
{
    private readonly ILogger<GenerateAudio> _logger;
    private readonly IEnumerable<IAudioGenerationService> _audioGenerationServices;

    public GenerateAudio(ILogger<GenerateAudio> logger, IEnumerable<IAudioGenerationService> audioGenerationServices)
    {
        _logger = logger;
        _audioGenerationServices = audioGenerationServices;
    }

    [Function("GenerateAudio")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        [FromBody] AudioGenerateDto dto)
    {
        var responseBytes = await GetAudio(dto.Input, dto.Separator, dto.AudioProviderName);

        return await req.GetFileResponseAsync(responseBytes);
    }

    private async Task<byte[]> GetAudio(string input, string separator, string providerName)
    {
        var service = _audioGenerationServices.FirstOrDefault(x =>
            x.Type.Equals(providerName, StringComparison.InvariantCultureIgnoreCase));

        if (service is null)
        {
            throw new AudioProviderNotSupported(providerName);
        }

        var articles = ContentAggregator.GetContentsForAudio(input, separator, 4095);

        return await service.GetAudio(articles);
    }
}