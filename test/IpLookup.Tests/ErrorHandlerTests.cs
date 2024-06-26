using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace IpLookup.Api.Tests;

public class ErrorHandlerTests
{
    [Fact]
    public void Handle_ReturnsBadRequest_WhenArgumentException()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Features.Set<IExceptionHandlerPathFeature>(new ExceptionHandlerFeature
        {
            Error = new ArgumentException("Test exception")
        });

        // Act
        var result = ErrorHandler.Handle(context);

        // Assert
        var json = Assert.IsType<JsonHttpResult<ProblemDetails>>(result);
        Assert.Equal((int)HttpStatusCode.BadRequest, json.StatusCode);
        Assert.Equal((int)HttpStatusCode.BadRequest, json.Value?.Status);
        Assert.Equal("BadRequest", json.Value?.Title);
        Assert.Equal("Test exception", json.Value?.Detail);
    }

    [Fact]
    public void Handle_ReturnsBadRequest_WhenFormatException()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Features.Set<IExceptionHandlerPathFeature>(new ExceptionHandlerFeature
        {
            Error = new FormatException("Test exception")
        });

        // Act
        var result = ErrorHandler.Handle(context);

        // Assert
        var json = Assert.IsType<JsonHttpResult<ProblemDetails>>(result);
        Assert.Equal((int)HttpStatusCode.BadRequest, json.StatusCode);
        Assert.Equal((int)HttpStatusCode.BadRequest, json.Value?.Status);
        Assert.Equal("BadRequest", json.Value?.Title);
        Assert.Equal("Test exception", json.Value?.Detail);
    }

    [Fact]
    public void Handle_ReturnsInternalServerError_WhenOtherException()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Features.Set<IExceptionHandlerPathFeature>(new ExceptionHandlerFeature
        {
            Error = new Exception("Test exception")
        });

        // Act
        var result = ErrorHandler.Handle(context);

        // Assert
        var json = Assert.IsType<JsonHttpResult<ProblemDetails>>(result);
        Assert.Equal((int)HttpStatusCode.InternalServerError, json.StatusCode);
        Assert.Equal((int)HttpStatusCode.InternalServerError, json.Value?.Status);
        Assert.Equal("InternalServerError", json.Value?.Title);
        Assert.Equal("Test exception", json.Value?.Detail);
    }

    [Fact]
    public void Handle_ReturnsInternalServerError_WhenNoException()
    {
        // Note: This should never happen.
        var context = new DefaultHttpContext();
        ErrorHandler.Handle(context);
    }
}