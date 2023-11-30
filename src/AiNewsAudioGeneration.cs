using System.Net;
using System.Net.Http.Headers;
using function.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace function
{
    public class AiNewsAudioGeneration
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;
        private readonly AiNewsOptions _options;

        public AiNewsAudioGeneration(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, IOptions<AiNewsOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _logger = loggerFactory.CreateLogger<AiNewsAudioGeneration>();
            _options = options.Value;
        }

        [Function("AiNewsAudioGeneration")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req, [FromBody] AudioGenerateDto dto)
        {
            var httpClient = GetClient();
            
            var audioResults = await Task.WhenAll(ContentAggregator.GetContentsForAudio(dto.Input, dto.Separator, 4095).Select(
                async x => await OpenAiClient.GetAudio(httpClient, x)));

            var responseBytes = audioResults.SelectMany(x => x).ToArray();
            
            return await req.GetFileResponseAsync(responseBytes);
        }

        private HttpClient GetClient()
        {
            var httpClient = _httpClientFactory.CreateClient("OpenAI");
            httpClient.BaseAddress = new Uri(_options.OpenAiUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.OpenAiKey);

            return httpClient;
        }
    }
}
