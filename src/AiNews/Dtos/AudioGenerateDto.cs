namespace AiNews.Dtos;

public record AudioGenerateDto
{
    public string Input { get; init; }
    public string Separator { get; init; }
    public string AudioProviderName { get; init; }
    public string AudioProviderPayload { get; init; }
}