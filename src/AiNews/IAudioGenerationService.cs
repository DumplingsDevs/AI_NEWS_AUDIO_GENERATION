namespace AiNews;

public interface IAudioGenerationService
{
    Task<byte[]> GetAudio(IEnumerable<string> contents);
    string Type { get; }
}