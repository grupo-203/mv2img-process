using FFMpegCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Movie2Image.Process.Application.Ports.Output.Media;

namespace Movie2Image.Process.Media.FFMPEG;

public class FfmpegMovieIntoImagesTransform(
	ILogger<FfmpegMovieIntoImagesTransform> logger,
    IConfiguration config) : IMovieIntoImagesTransform
{

    private readonly int spf = int.Parse(config["SECONDS_PER_FRAME"] ?? "1");

    public async Task Transform(string moviePath, string framesPath)
    {
		var totalSeconds = await GetTimeInSeconds(moviePath);

		if (!Directory.Exists(framesPath))
			Directory.CreateDirectory(framesPath);

		for (int second = 0; second < totalSeconds; second += spf)
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

}
