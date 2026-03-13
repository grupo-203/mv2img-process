namespace Movie2Image.Process.Application.Ports.Output.ExternalServices;

public interface IDeliveryService
{

    Task Request(string file, string userId);

}
