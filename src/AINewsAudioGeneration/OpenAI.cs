namespace AINewsAudioGeneration;

internal static class OpenAiClient
{
    public static async Task<byte[]> GetAudio(HttpClient httpClient, string input)
    {
        var requestData = new
        {
            //TODO: appsettings config?
            model = "tts-1-hd",
            voice = "nova",
            input,
        };

        //TODO: Uri from appsettings
        using var response = await httpClient.PostAsJsonAsync("/v1/audio/speech", requestData);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsByteArrayAsync();
        }

        //TODO: Dedicated exception
        throw new Exception("Error");
    }
}
