using System.Net.Http.Headers;
using System.Text;
using AINewsAudioGeneration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

var app = builder.Build();

app.MapPost("/audio/generate", async (AudioGenerateDto dto, IHttpClientFactory httpClientFactory) =>
{
    var httpClient = httpClientFactory.CreateClient("OpenAI");
    httpClient.BaseAddress = new Uri("https://api.openai.com");
    string apiKey = ""; //TODO: Get from appsettings
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

    var audioResults = await Task.WhenAll(ContentAggregator.GetContentsForAudio(dto.Input, dto.Separator, 4095).Select(
        async x => await OpenAiClient.GetAudio(httpClient, x)));

    var stream = new MemoryStream();

    foreach (var audioBytes in audioResults)
    {
        stream.Write(audioBytes, 0, audioBytes.Length);
    }

    stream.Seek(0, SeekOrigin.Begin);
    
    return Results.File(stream, fileDownloadName: "output.flac");
});

app.Run();