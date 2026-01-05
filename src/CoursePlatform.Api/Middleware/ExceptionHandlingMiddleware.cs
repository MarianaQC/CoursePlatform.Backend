using System.Net;
using System.Text.Json;
using CoursePlatform.Application.Common;
using FluentValidation;

namespace CoursePlatform.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Ha ocurrido un error no controlado: {Message}", exception.Message);

        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            ValidationException validationException => CreateValidationErrorResponse(context, validationException),
            UnauthorizedAccessException _ => CreateUnauthorizedResponse(context),
            _ => CreateInternalServerErrorResponse(context, exception)
        };

        var jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }

    private static ApiResponse<object> CreateValidationErrorResponse(HttpContext context, ValidationException exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        
        var errors = exception.Errors
            .Select(e => e.ErrorMessage)
            .ToList();

        return ApiResponse<object>.Fail(errors);
    }

    private static ApiResponse<object> CreateUnauthorizedResponse(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        return ApiResponse<object>.Fail("No autorizado para realizar esta operación.");
    }

    private static ApiResponse<object> CreateInternalServerErrorResponse(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        
        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        var message = isDevelopment 
            ? $"Error interno del servidor: {exception.Message}" 
            : "Ha ocurrido un error interno del servidor.";

        return ApiResponse<object>.Fail(message);
    }
}