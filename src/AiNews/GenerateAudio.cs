using AiNews.Dtos;
using AiNews.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace AiNews;

public class GenerateAudio
{
    private readonly IEnumerable<IAudioGenerationService> _audioGenerationServices;

    public GenerateAudio(IEnumerable<IAudioGenerationService> audioGenerationServices)
    {
        _audioGenerationServices = audioGenerationServices;
    }
   
    [Function("GenerateAudio")]
    public async Task<FileContentResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        var dto = await req.ReadFromJsonAsync<AudioGenerateDto>();
        var generationResult =
            await GetAudio(dto.Input, dto.Separator, dto.AudioProviderName, dto.AudioProviderPayload);

        return FileResult(generationResult);
    }

    private async Task<AudioGenerationResult> GetAudio(string input, string separator, string providerName,
        object providerPayload)
    {
        var service = _audioGenerationServices.FirstOrDefault(x =>
            x.Type.Equals(providerName, StringComparison.InvariantCultureIgnoreCase));

        if (service is null)
        {
            throw new AudioProviderNotSupported(providerName);
        }

        return await service.GetAudio(maxChunk => ContentAggregator.GetContentsForAudio(input, separator, maxChunk),
            providerPayload);
    }

    private static FileContentResult FileResult(AudioGenerationResult generationResult)
    {
        return new FileContentResult(generationResult.Audio, "application/octet-stream")
        {
            FileDownloadName = $"output_{DateTime.Now:HH_mm_ss_d_M_y}.{generationResult.Format}"
        };
    }
}