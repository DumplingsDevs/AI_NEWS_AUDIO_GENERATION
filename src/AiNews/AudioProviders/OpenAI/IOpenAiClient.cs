namespace AiNews.AudioProviders.OpenAI;

public interface IOpenAiClient
{
    Task<byte[]> GetAudio(string input);
}