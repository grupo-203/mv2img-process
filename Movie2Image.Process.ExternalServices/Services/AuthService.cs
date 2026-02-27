using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Ports.Output.ExternalServices;
using Movie2Image.Process.ExternalServices.Model;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;

namespace Movie2Image.Process.ExternalServices.Services;

public class AuthService(
	IMemoryCache cache,
	IConfiguration config) : IAuthService
{

	private readonly string baseUrl = config["AUTH_SERVICE_URL"]
		?? throw new ArgumentNullException("AUTH_SERVICE_URL");

	private readonly string clientId = config["CLIENT_ID"] 
		?? throw new ArgumentNullException("CLIENT_ID");

	private readonly string clientSecret = config["CLIENT_SECRET"]
		?? throw new ArgumentNullException("CLIENT_SECRET");


	public async Task<AuthenticationHeaderValue> Login()
	{
		var token = await GetToken();

		return new AuthenticationHeaderValue("Bearer", token);
	}

	public async Task<UserDto> GetUser(string id)
	{
		using var http = new HttpClient();

		var message = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/user/{id}");

		message.Headers.Authorization = await Login();

		var response = await http.SendAsync(message);

		response.EnsureSuccessStatusCode();

		var scontent = await response.Content.ReadAsStringAsync();

		return JsonConvert.DeserializeObject<UserDto>(scontent)
			?? throw new Exception("Failed to deserialize user response");
	}


	private async Task<string> GetToken()
	{
		var token = await cache.GetOrCreateAsync("auth:token", async entry =>
		{
			var token = await DoLogin();

			if (!IsValidToken(token))
				throw new Exception("Invalid access token");

			entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(token.expires_in!.Value);

			return token;
		});

		return token!.access_token!;
	}

	private async Task<TokenDto> DoLogin()
	{
		using var client = new HttpClient();

		var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/oauth/token");

		request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
		{
			{ "grant_type", "client_credentials" },
			{ "client_id", clientId },
			{ "client_secret", clientSecret }
		});

		var response = await client.SendAsync(request);

		response.EnsureSuccessStatusCode();

		var scontent = await response.Content.ReadAsStringAsync();

		return JsonConvert.DeserializeObject<TokenDto>(scontent)
			?? throw new Exception("Failed to deserialize token response");
	}

	private bool IsValidToken([NotNullWhen(true)]TokenDto? token)
	{
		return
			token != null &&
			token.expires_in.HasValue &&
			!string.IsNullOrEmpty(token.access_token);
	}

}
