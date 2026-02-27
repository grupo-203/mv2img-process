using Movie2Image.Process.Application.Ports.Output.ExternalServices;

namespace Movie2Image.Process.ExternalServices.Mock;

public class LoadMock : ILoadService
{

	public Task ErrorProcess(string id) => Task.CompletedTask;

	public Task FinishProcess(string id, string zipPath) => Task.CompletedTask;

	public Task StartProcess(string id) => Task.CompletedTask;

}
