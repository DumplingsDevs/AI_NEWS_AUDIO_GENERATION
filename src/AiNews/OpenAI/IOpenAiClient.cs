namespace AiNews.OpenAI;

public interface IOpenAiClient
{
    Task<byte[]> GetAudio(string input);
}