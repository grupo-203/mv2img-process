using Movie2Image.Process.Application.Ports.Output.Storage;

namespace Movie2Image.Process.Storage.Basic;

public class FileSystemStorage : IStorage
{

    public async Task<string> UploadFile(Stream file, string path)
    {
        using var destination = File.OpenWrite(path);

        await file.CopyToAsync(destination);

        return path;
	}

}
