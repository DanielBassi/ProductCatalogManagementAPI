using Azure.Core;
using Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Api.Middleware;


public class AppExceptionHandlerMiddleware(RequestDelegate next, ILogger<AppExceptionHandlerMiddleware> logger)
{
    private static void CheckHttpContext(HttpContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }
    }

    public async Task InvokeAsync(HttpContext context)
    {
        CheckHttpContext(context);

        try
        {
            await next.Invoke(context);
        }
        catch (Exception ex) when (ex is FileNotFoundException || ex is IOException || ex is CoreBusinessException)
        {
            await LogAndHandleExceptionAsync(context, ex);
        }
    }

    private async Task LogAndHandleExceptionAsync(HttpContext context, Exception ex)
    {
        logger.LogWarning(Resources.errorLog, "{ErrorMessage}", ex.Message);

        var result = JsonSerializer.Serialize(new
        {
            ErrorMessage = ex.Message
        });

        context.Response.ContentType = ContentType.ApplicationJson.ToString();
        context.Response.StatusCode =
            ex is CoreBusinessException ? (int)HttpStatusCode.BadRequest : (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsync(result);
    }
}