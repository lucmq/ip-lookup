using Microsoft.AspNetCore.Mvc.Testing;

namespace IpLookup.Api.Tests;

public class ProgramTests
{
    // For more on integration testing, for the Program class, see:
    // https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests

    [Theory]
    [InlineData("/")]
    [InlineData("/lookup/1.0.0.0")]
    public async Task Main_ShouldRunApplication(string url)
    {
        // Arrange
        Environment.SetEnvironmentVariable("ImportTask__FileUri",
                                           TestData.DbIpCityIpv4Filepath);
        var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("application/json; charset=utf-8",
                     response.Content.Headers.ContentType?.ToString());

        await factory.DisposeAsync();
    }
}