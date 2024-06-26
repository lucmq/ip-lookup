using System.Net;
using Moq;
using Moq.Protected;

namespace IpLookup.Api.Tests;

/// <summary>
/// Provides a set of test data.
/// </summary>
public static class TestData
{
    private static readonly string DataFolder =
        Path.GetFullPath(Path.Join("..", "..", "..", "TestData"));

    /// <summary>
    /// The path to the test CSV file containing the DBIP City IPv4 data.
    /// </summary>
    public static readonly string DbIpCityIpv4Filepath =
        Path.Join(DataFolder, "dbip-city-ipv4-test.csv");

    /// <summary>
    /// The path to the test GZipped CSV file containing the DBIP City IPv4 data.
    /// </summary>
    public static readonly string DbIpCityIpv4GzipFilepath =
        Path.Join(DataFolder, "dbip-city-ipv4-test.csv.gz");

    /// <summary>
    /// Creates a <see cref="StreamReader"/> for the test CSV file.
    /// </summary>
    /// <returns>The <see cref="StreamReader"/>.</returns>
    public static StreamReader DbIpCityIpv4Reader()
    {
        var absolutePath = Path.GetFullPath(DbIpCityIpv4Filepath);
        return new StreamReader(absolutePath);
    }

    /// <summary>
    /// Creates a <see cref="StreamReader"/> for the test GZipped CSV file.
    /// </summary>
    /// <returns>The <see cref="StreamReader"/>.</returns>
    public static StreamReader DbIpCityIpv4GzipReader()
    {
        var absolutePath = Path.GetFullPath(DbIpCityIpv4GzipFilepath);
        return new StreamReader(absolutePath);
    }

    /// <summary>
    /// Provides a mocked <see cref="IHttpClientFactory"/> for the test CSV file.
    /// </summary>
    /// <returns></returns>
    public static Mock<IHttpClientFactory> DbIpCityIpv4RemoteMock()
    {
        var reader = DbIpCityIpv4Reader();
        var handlerMock = HttpMessageHandlerMock(reader.BaseStream);
        return HttpClientFactoryMock(handlerMock.Object);
    }

    /// <summary>
    /// Provides a mocked <see cref="IHttpClientFactory"/> for the test GZipped
    /// CSV file.
    /// </summary>
    /// <returns>The <see cref="IHttpClientFactory"/>.</returns>
    public static Mock<IHttpClientFactory> DbIpCityIpv4GzipRemoteMock()
    {
        var reader = DbIpCityIpv4GzipReader();
        var handlerMock = HttpMessageHandlerMock(reader.BaseStream);
        return HttpClientFactoryMock(handlerMock.Object);
    }

    // Helpers

    private static Mock<IHttpClientFactory> HttpClientFactoryMock(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler);
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock.Setup(factory => factory.CreateClient(It.IsAny<string>()))
                             .Returns(httpClient);
        return httpClientFactoryMock;
    }

    private static Mock<HttpMessageHandler> HttpMessageHandlerMock(Stream stream)
    {
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StreamContent(stream)
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        return handlerMock;
    }
}