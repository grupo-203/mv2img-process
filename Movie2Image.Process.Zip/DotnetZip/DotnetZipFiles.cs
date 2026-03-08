using Microsoft.Extensions.Logging;
using Movie2Image.Process.Application.Ports.Output.Zip;
using Movie2Image.Process.Domain.Validation;
using System.IO.Compression;

namespace Movie2Image.Process.Zip.DotnetZip;

public class DotnetZipFiles(
	ILogger<DotnetZipFiles> logger) : IZipCreate
{

    public async Task Create(string zipPath, string zipName)
    {
		Validator.Create()
			.Test(!string.IsNullOrEmpty(zipPath), "Invalid Zip Path")
			.Test(!string.IsNullOrEmpty(zipName), "Invalid Zip Name")
			.Validate();

		using var stream = new FileStream(zipName, FileMode.Create);
		using var zip = new ZipArchive(stream, ZipArchiveMode.Create);

		foreach (var file in Directory.GetFiles(zipPath))
		{
			if (file == zipName)
				continue;

			var relativePath = Path.GetRelativePath(zipPath, file);

			zip.CreateEntryFromFile(file, relativePath, CompressionLevel.Optimal);

			logger.LogInformation("Added [{file}] to [{zipName}]", file, zipName);
		}
	}

}
