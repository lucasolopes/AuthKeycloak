using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuthKeycloak.Extensions;

public class TokenService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public TokenService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<TokenResponse> GetTokenAsync(string username, string password)
    {
        var keycloakSettings = _configuration.GetSection("Keycloak");
        var realm = keycloakSettings["realm"];
        var clientId = keycloakSettings["resource"];
        var clientSecret = keycloakSettings["credentials:secret"];
        var authServerUrl = keycloakSettings["auth-server-url"];

        var tokenEndpoint = $"{authServerUrl.TrimEnd('/')}/realms/{realm}/protocol/openid-connect/token";

        var requestData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string, string>("password", password)
        });

        var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
        {
            Content = requestData
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TokenResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public async Task<TokenResponse> GetClientCredentialsTokenAsync()
    {
        var keycloakSettings = _configuration.GetSection("Keycloak");
        var realm = keycloakSettings["realm"];
        var clientId = keycloakSettings["resource"];
        var clientSecret = keycloakSettings["credentials:secret"];
        var authServerUrl = keycloakSettings["auth-server-url"];

        var tokenEndpoint = $"{authServerUrl.TrimEnd('/')}/realms/{realm}/protocol/openid-connect/token";

        var requestData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret)
        });

        var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
        {
            Content = requestData
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TokenResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}

public class TokenResponse
{
    [JsonPropertyName("access_token")] public string AccessToken { get; set; }

    [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; }

    [JsonPropertyName("refresh_expires_in")]
    public int RefreshExpiresIn { get; set; }

    [JsonPropertyName("token_type")] public string TokenType { get; set; }

    [JsonPropertyName("id_token")] public string IdToken { get; set; }

    [JsonPropertyName("session_state")] public string SessionState { get; set; }

    [JsonPropertyName("scope")] public string Scope { get; set; }
}