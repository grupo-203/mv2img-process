namespace Movie2Image.Process.Application.Ports.Output.Storage;

public interface IStorage
{

	Task<string> UploadFile(Stream file, string path);

}
