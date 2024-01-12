using AiNews.Dtos;
using AiNews.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace AiNews;

public class TriggerWarmup
{
    [Function("TriggerWarmup")]
    public IResult Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        return Results.Ok("Warmed up");
    }
    
}