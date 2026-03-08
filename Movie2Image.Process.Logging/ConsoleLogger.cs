using Microsoft.Extensions.Logging;

namespace Movie2Image.Process.Logging;

public static class ConsoleLogger
{
	public static void LogDebug(string message) =>
		Log(LogLevel.Debug, message);

	public static void LogInformation(string message) =>
		Log(LogLevel.Information, message);

	public static void LogWarning(string message) =>
		Log(LogLevel.Warning, message);

	public static void LogError(string message, Exception? ex = null) =>
		Log(LogLevel.Error, message, ex);

	public static void LogCritical(string message, Exception? ex = null) =>
		Log(LogLevel.Critical, message, ex);


	private static void Log(LogLevel level, string message, Exception? ex = null)
	{
		var textWriter = Console.Out;

		ConsoleLogFormatter.WriteDate(textWriter);
		ConsoleLogFormatter.WriteLevel(textWriter, LogLevel.Information);
		ConsoleLogFormatter.WriteMessage(textWriter, message, ex);
	}

}
