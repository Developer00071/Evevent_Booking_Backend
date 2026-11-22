using Application;

namespace WebApi.Middlewares;

public class ApiExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ApiException exception)
        {
            context.Response.StatusCode = exception.StatusCode;
            await Results.Problem(
                statusCode: exception.StatusCode,
                title: exception.Message).ExecuteAsync(context);
        }
    }
}
