namespace Movie2Image.Process.Application.Ports.Output.ExternalServices;

public interface ILoadService
{

	Task StartProcess(string id);

	Task FinishProcess(string id, string zipPath);

	Task ErrorProcess(string id);

}
