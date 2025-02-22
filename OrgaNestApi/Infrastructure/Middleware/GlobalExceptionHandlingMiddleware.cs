namespace OrgaNestApi.Infrastructure.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred");

            context.Response.ContentType = "application/json";
            var statusCode = StatusCodes.Status500InternalServerError;
            var message = "An unexpected error occurred";

            switch (ex)
            {
                case ArgumentException _:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = ex.Message;
                    break;
                case InvalidOperationException _:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = ex.Message;
                    break;
                case UnauthorizedAccessException _:
                    statusCode = StatusCodes.Status403Forbidden;
                    message = "Access denied";
                    break;
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    break;
            }

            context.Response.StatusCode = statusCode;
            var errorResponse = new { message };
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}