using System.Net;
using Microsoft.Azure.Functions.Worker.Http;

namespace AiNews.Extensions;

public static class HttpRequestDataExtensions
{
    public static async Task<HttpResponseData> GetFileResponseAsync(this HttpRequestData requestData, byte[] bytes, string fileExtension)
    {
        var response = requestData.CreateResponse(HttpStatusCode.OK);
        await response.WriteBytesAsync(bytes);
        response.Headers.Add("Content-type", "application/octet-stream");
        response.Headers.Add("content-disposition", $"attachment;filename=output_{DateTime.Now:HH_mm_ss_d_M_y}.{fileExtension}");

        return response;
    }
}
