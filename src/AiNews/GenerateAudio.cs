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
    public async Task<FileContentResult> Run([HttpTrigger(AuthorizationLevel.Admin, "post")] HttpRequest req)
    {
        AudioGenerateDto dto = await AudioGenerateDto.CreateAsync(req);
        
        var generationResult = await GetAudio(dto.Input, dto.Separator, dto.AudioProviderName, dto.AudioProviderPayload);
        var audioWithIntro = new List<byte[]> { dto.Intro, generationResult.Audio };
        var completedAudio = audioWithIntro.SelectMany(x => x).ToArray();

        return new FileContentResult(completedAudio, "application/octet-stream")
        {
            FileDownloadName = $"output_{DateTime.Now:HH_mm_ss_d_M_y}.{generationResult.Format}"
        };
    }

    private async Task<AudioGenerationResult> GetAudio(string input, string separator, string providerName, string providerPayload)
    {
        var service = _audioGenerationServices.FirstOrDefault(x =>
            x.Type.Equals(providerName, StringComparison.InvariantCultureIgnoreCase));

        if (service is null)
        {
            throw new AudioProviderNotSupported(providerName);
        }

        return await service.GetAudio(maxChunk => ContentAggregator.GetContentsForAudio(input, separator, maxChunk), providerPayload);
    }
}