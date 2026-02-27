namespace Movie2Image.Process.Application.Ports.Output.Zip;

public interface IZipCreate
{

	Task Create(string zipPath, string zipName);

}
