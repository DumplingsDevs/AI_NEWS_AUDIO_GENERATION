using System.Net.Http.Json;

namespace function;

internal static class OpenAiClient
{
    public static async Task<byte[]> GetAudio(HttpClient httpClient, string input)
    {
        var requestData = new
        {
            model = "tts-1-hd",
            voice = "nova",
            input,
        };

        using var response = await httpClient.PostAsJsonAsync("/v1/audio/speech", requestData);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsByteArrayAsync();
        }

        throw new Exception($"ErrorCode from OpenAI: {response.StatusCode}");
    }
}
