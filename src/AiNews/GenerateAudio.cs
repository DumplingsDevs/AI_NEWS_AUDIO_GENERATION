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
    private readonly IEnumerable<IAudioGenerationService> _audioGenerationServices;

    public GenerateAudio(IEnumerable<IAudioGenerationService> audioGenerationServices)
    {
        _audioGenerationServices = audioGenerationServices;
    }

    [Function("GenerateAudio")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Admin, "post")] HttpRequestData req,
        [FromBody] AudioGenerateDto dto)
    {
        var generationResult = await GetAudio(dto.Input, dto.Separator, dto.AudioProviderName);

        return await req.GetFileResponseAsync(generationResult.Audio, generationResult.Format);
    }

    private async Task<AudioGenerationResult> GetAudio(string input, string separator, string providerName)
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