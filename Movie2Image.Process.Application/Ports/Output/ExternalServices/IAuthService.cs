using Movie2Image.Process.Application.DTO;
using System.Net.Http.Headers;

namespace Movie2Image.Process.Application.Ports.Output.ExternalServices;

public interface IAuthService
{

	Task<AuthenticationHeaderValue> Login();

	Task<UserDto> GetUser(string id);

}
