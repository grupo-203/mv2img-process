using Movie2Image.Process.Application.Ports.Input;
using Movie2Image.Process.Application.Ports.Output.ExternalServices;
using System.Net.Http.Json;

namespace Movie2Image.Process.ExternalServices.Services;

public class LoadService(
	IAuthService auth,
	IProcessConfiguration config) : ILoadService
{

	public async Task StartProcess(string id)
	{
		using var http = new HttpClient();

		var message = new HttpRequestMessage(HttpMethod.Post, $"{config.LoadServiceUrl}/start");

		message.Headers.Authorization = await auth.Login();
		message.Content = JsonContent.Create(new { id });

		var response = await http.SendAsync(message);

		response.EnsureSuccessStatusCode();
	}

	public async Task FinishProcess(string id, string zipPath)
	{
		using var http = new HttpClient();

		var message = new HttpRequestMessage(HttpMethod.Post, $"{config.LoadServiceUrl}/finish");

		message.Headers.Authorization = await auth.Login();
		message.Content = JsonContent.Create(new { id, zipPath });

		var response = await http.SendAsync(message);

		response.EnsureSuccessStatusCode();
	}

	public async Task ErrorProcess(string id)
	{
		using var http = new HttpClient();

		var message = new HttpRequestMessage(HttpMethod.Post, $"{config.LoadServiceUrl}/error");
		message.Headers.Authorization = await auth.Login();
		message.Content = JsonContent.Create(new { id });

		var response = await http.SendAsync(message);

		response.EnsureSuccessStatusCode();
	}

}
