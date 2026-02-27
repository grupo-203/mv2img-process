using Microsoft.Extensions.Configuration;
using Movie2Image.Process.Application.Ports.Output.ExternalServices;
using System.Net.Http.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Movie2Image.Process.ExternalServices.Services;

public class LoadService(
	IAuthService auth,
	IConfiguration config) : ILoadService
{

	private readonly string baseUrl = config["LOAD_SERVICE_URL"] 
		?? throw new ArgumentNullException("LOAD_SERVICE_URL");

	public async Task StartProcess(string id)
	{
		using var http = new HttpClient();

		var message = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/start");

		message.Headers.Authorization = await auth.Login();
		message.Content = JsonContent.Create(new { id });

		var response = await http.SendAsync(message);

		response.EnsureSuccessStatusCode();
	}

	public async Task FinishProcess(string id, string zipPath)
	{
		using var http = new HttpClient();

		var message = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/finish");

		message.Headers.Authorization = await auth.Login();
		message.Content = JsonContent.Create(new { id, zipPath });

		var response = await http.SendAsync(message);

		response.EnsureSuccessStatusCode();
	}

	public async Task ErrorProcess(string id)
	{
		using var http = new HttpClient();

		var message = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/error");
		message.Headers.Authorization = await auth.Login();
		message.Content = JsonContent.Create(new { id });

		var response = await http.SendAsync(message);

		response.EnsureSuccessStatusCode();
	}

}
