using System.Net;
using System.Text.Json;
using PasswordGenerator.Api.Data;
using Xunit;

namespace PasswordGenerator.IntegrationTests;

public sealed class PasswordsApiIntegrationTests
{
    [Fact]
    public async Task Post_CreatesPasswordAndPersistsItBeforeReturningCreated()
    {
        using var factory = await PasswordApiFactory.CreateInitializedAsync();
        using var client = factory.CreateClient();

        using var response = await client.PostAsync("/api/passwords", content: null);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var idText = body.RootElement.GetProperty("id").GetString();
        Assert.True(Guid.TryParseExact(idText, "D", out var id));

        var location = response.Headers.Location;
        Assert.NotNull(location);
        var locationPath = location!.IsAbsoluteUri ? location.AbsolutePath : location.OriginalString;
        Assert.Equal($"/api/passwords/{id:D}", locationPath);

        var persistedRecord = await factory.FindRecordAsync(id);
        Assert.NotNull(persistedRecord);
        Assert.Equal(id, persistedRecord!.Id);
        Assert.False(string.IsNullOrWhiteSpace(persistedRecord.Password));
    }

    [Fact]
    public async Task Get_StoredPassword_ReturnsThePersistedValue()
    {
        using var factory = await PasswordApiFactory.CreateInitializedAsync();
        using var client = factory.CreateClient();
        var id = Guid.NewGuid();
        const string expectedPassword = "literal-stored-password-42!";
        await factory.SeedRecordAsync(id, expectedPassword);

        using var response = await client.GetAsync($"/api/passwords/{id:D}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(id.ToString("D"), body.RootElement.GetProperty("id").GetString());
        Assert.Equal(expectedPassword, body.RootElement.GetProperty("password").GetString());
    }

    [Fact]
    public async Task Get_MalformedId_ReturnsBadRequestProblemDetailsWithIdErrors()
    {
        using var factory = await PasswordApiFactory.CreateInitializedAsync();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/passwords/not-a-guid");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var problem = body.RootElement;
        Assert.Equal((int)HttpStatusCode.BadRequest, problem.GetProperty("status").GetInt32());
        Assert.True(problem.GetProperty("errors").GetProperty("id").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Get_ValidIdWithoutRecord_ReturnsNotFoundProblemDetails()
    {
        using var factory = await PasswordApiFactory.CreateInitializedAsync();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync($"/api/passwords/{Guid.NewGuid():D}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var problem = body.RootElement;
        Assert.Equal((int)HttpStatusCode.NotFound, problem.GetProperty("status").GetInt32());
        Assert.True(problem.TryGetProperty("type", out _));
        Assert.True(problem.TryGetProperty("title", out _));
        Assert.True(problem.TryGetProperty("detail", out _));
        Assert.True(problem.TryGetProperty("instance", out _));
    }
}