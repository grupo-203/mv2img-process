using FFMpegCore;
using Microsoft.Extensions.Logging;
using Movie2Image.Process.Application.Ports.Input;
using Movie2Image.Process.Application.Ports.Output.Media;

namespace Movie2Image.Process.Media.FFMPEG;

public class FfmpegMovieIntoImagesTransform(
	ILogger<FfmpegMovieIntoImagesTransform> logger,
    IProcessConfiguration config) : IMovieIntoImagesTransform
{

    public async Task Transform(string moviePath, string framesPath)
    {
		var totalSeconds = await GetTimeInSeconds(moviePath);

		CreatePath(framesPath);

        for (int second = 0; second < totalSeconds; second += config.SecondsPerFrame)
		{
			var snapshotFilename = Path.Combine(framesPath, $"snapshot_{second:D5}.jpg");

			await CreateSnapshot(moviePath, snapshotFilename, second);
		}
	}


	private async Task CreateSnapshot (string inputFile, string outputFile, int second)
	{
		logger.LogInformation("Creating snapshot for [{inputFile}] at second [{second}]", inputFile, second);

		await FFMpegArguments
			.FromFileInput(inputFile, verifyExists: false, options => options
				.Seek(TimeSpan.FromSeconds(second)))
			.OutputToFile(outputFile, overwrite: true, options => options
				.WithFrameOutputCount(1)
				.WithCustomArgument("-update 1"))
			.ProcessAsynchronously();
	}

	private async Task<int> GetTimeInSeconds(string moviePath)
	{
		var mediaInfo = await FFProbe.AnalyseAsync(moviePath);
		var totalSeconds = mediaInfo.Duration.TotalSeconds;

		return Convert.ToInt32(totalSeconds);
	}

	private void CreatePath(string path)
	{
		logger.LogInformation("Checking path [{path}]", path);

		if (Directory.Exists(path))
			return;

		logger.LogInformation("Creating path [{path}]", path);

        Directory.CreateDirectory(path);
    }

}
