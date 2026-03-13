using Movie2Image.Process.Application.Ports.Input;
using Movie2Image.Process.Application.Ports.Output.ExternalServices;
using System.Net.Http.Json;

namespace Movie2Image.Process.ExternalServices.Services;

public class DeliveryService(
    IAuthService authService,
    IProcessConfiguration config) : IDeliveryService
{

    public async Task Request(string file, string userId)
    {
        using var http = new HttpClient();

        var message = new HttpRequestMessage(HttpMethod.Post, 
            $"{config.DeliveryServiceUrl}/api/downloads/request/{file}");

        //message.Headers.Authorization = await authService.Login();
        message.Content = JsonContent.Create(new
        {
            user_id = userId,
            file_name = file
        });

        var response = await http.SendAsync(message);

        response.EnsureSuccessStatusCode();
    }

}
