using Microsoft.Extensions.Logging;
using Movie2Image.Process.Application.Ports.Output.Storage;

namespace Movie2Image.Process.Storage.Basic;

public class FileSystemStorage(
    ILogger<FileSystemStorage> logger) : IStorage
{

    public async Task<string> UploadFile(Stream file, string path)
    {
        CreateDirectory(path);

        using var destination = File.OpenWrite(path);

        await file.CopyToAsync(destination);

        return path;
	}

    private void CreateDirectory(string path)
    {
        var directory = Path.GetDirectoryName(path);

        logger.LogInformation("Check directory [{directory}]", directory);

        if (Directory.Exists(directory))
            return;

        Directory.CreateDirectory(directory!);

        logger.LogInformation("Creating directory [{directory}]", directory);
    }

}
