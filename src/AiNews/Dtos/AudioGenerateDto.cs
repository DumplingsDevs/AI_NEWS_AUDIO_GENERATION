using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace AiNews.Dtos;

public record AudioGenerateDto
{
    public string Input { get; init; }
    public string Separator { get; init; }
    public string AudioProviderName { get; init; }
    public string AudioProviderPayload { get; init; }
    public byte[] Intro { get; init; }

    public static async Task<AudioGenerateDto> CreateAsync(HttpRequest request)
    {
        var formData = await request.ReadFormAsync();
        var introFile = formData.Files.FirstOrDefault(x => x.Name == "intro");
        using var stream = new MemoryStream();
        
        await introFile.CopyToAsync(stream);

        return new AudioGenerateDto()
        {
            Input = formData["input"],
            Separator = formData["separator"],
            AudioProviderName = formData["audioProviderName"],
            AudioProviderPayload = formData["audioProviderPayload"],
            Intro = stream.ToArray()
        };
    }
}