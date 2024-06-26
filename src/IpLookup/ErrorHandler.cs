using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace IpLookup.Api;

/// <summary>
/// The handler for errors in the application.
/// </summary>
public static class ErrorHandler
{
    /// <summary>
    /// The endpoint for the error handler.
    /// </summary>
    public const string Endpoint = "/error";

    /// <summary>
    /// Maps the IP lookup endpoints.
    /// </summary>
    /// <param name="app">The web application.</param>
    public static void MapErrorHandler(this WebApplication app)
    {
        app.MapGet(Endpoint, Handle);
    }

    /// <summary>
    /// Handles the error.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>An <see cref="IResult"/> containing the problem details.</returns>
    public static IResult Handle(HttpContext context)
    {
        var exceptionHandlerPathFeature =
            context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        var code = exception switch
        {
            ArgumentException _ => HttpStatusCode.BadRequest,
            FormatException _ => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

        // Note: It is not necessary to handle 'Not Found' here because the 'Get'
        // methods if the index return a boolean to indicate if the IP exists. 

        var problemDetails = new ProblemDetails
        {
            Status = (int)code,
            Title = code.ToString(),
            Detail = exception?.Message
        };
        return Results.Json(problemDetails, statusCode: (int)code);
    }
}