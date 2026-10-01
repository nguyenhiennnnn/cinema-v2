using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Cinema.IntegrationTests.Infrastructure;

namespace Cinema.IntegrationTests;

[Collection(CinemaApiCollection.Name)]
public abstract class IntegrationTestBase(CinemaApiFactory factory) : IAsyncLifetime
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    protected CinemaApiFactory Factory { get; } = factory;
    protected HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await Factory.ResetDatabaseAsync();
        Client = Factory.CreateClient();
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }

    protected static async Task<T> ReadDataAsync<T>(HttpResponseMessage response)
    {
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);
        var data = doc.RootElement.GetProperty("data");
        return data.Deserialize<T>(JsonOptions)!;
    }

    protected static async Task<JsonElement> ReadEnvelopeAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(body).RootElement.Clone();
    }

    protected async Task<(string AccessToken, int UserId)> RegisterConfirmedCustomerAndLoginAsync()
    {
        var (userId, email, password) =
            await IntegrationTestDataSeeder.CreateConfirmedCustomerAsync(Factory.Services);
        var accessToken = await LoginAsync(email, password);
        return (accessToken, userId);
    }

    protected async Task<string> LoginAsync(string email, string password, bool admin = false)
    {
        var endpoint = admin ? "/api/v1/auth/admin-login" : "/api/v1/auth/login";
        var loginResponse = await Client.PostAsJsonAsync(
            endpoint,
            new { email, password }
        );
        loginResponse.EnsureSuccessStatusCode();
        var login = await ReadDataAsync<LoginResponseDto>(loginResponse);
        return login.AccessToken;
    }

    protected HttpClient CreateAuthenticatedClient(string accessToken)
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            accessToken
        );
        return client;
    }

    protected record LoginResponseDto(
        string AccessToken,
        string RefreshToken,
        int ExpiresIn,
        LoginUserDto User
    );

    protected record LoginUserDto(
        int UserId,
        string FullName,
        string Email,
        string Role,
        string? Avatar,
        string? MembershipTierName,
        int? Points,
        bool IsEmailConfirmed
    );
}
