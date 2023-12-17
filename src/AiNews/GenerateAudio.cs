using AiNews.Dtos;
using AiNews.Exceptions;
using AiNews.Extensions;
using AiNews.OpenAI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IResult> Run([HttpTrigger(AuthorizationLevel.Admin, "post")] HttpRequest req)
    {
        AudioGenerateDto dto = await AudioGenerateDto.CreateAsync(req);
        return Results.Ok();
        // var generationResult = await GetAudio("dto.Input", "dto.Separator", "dto.AudioProviderName");

        // return await req.GetFileResponseAsync(generationResult.Audio, generationResult.Format);
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