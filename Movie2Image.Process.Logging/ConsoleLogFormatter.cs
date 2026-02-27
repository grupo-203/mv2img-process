using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Movie2Image.Process.Logging;

public class ConsoleLogFormatter : ConsoleFormatter
{
	public ConsoleLogFormatter() : base(nameof(ConsoleLogFormatter)) { }

	public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
	{
		WriteDate(textWriter);
		WriteLevel(textWriter, logEntry.LogLevel);
		WriteCategory(textWriter, logEntry.Category);
		WriteMessage(textWriter, logEntry.State, logEntry.Exception);
	}


	public static void WriteDate(TextWriter textWriter)
	{
		textWriter.Write($"[{DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}] ");
	}

	public static void WriteLevel(TextWriter textWriter, LogLevel level)
	{
		textWriter.Write(level switch
		{
			LogLevel.Trace => "\x1b[37mTrace", // Gray  
			LogLevel.Debug => "\x1b[36mDebug", // Cyan  
			LogLevel.Information => "\x1b[32mInformation", // Green  
			LogLevel.Warning => "\x1b[33mWarning", // Yellow  
			LogLevel.Error => "\x1b[31mError", // Red  
			LogLevel.Critical => "\x1b[41mCritical", // Red background  
			LogLevel.None => "\x1b[0mNone", // Reset  
			_ => "\x1b[0mUnknown" // Reset for unknown levels  
		});

		textWriter.Write("\u001b[0m: ");
	}

	public static void WriteCategory(TextWriter textWriter, string category)
	{
		textWriter.Write($"({category}) ");
	}

	public static void WriteMessage<TState>(TextWriter textWriter, TState message, Exception? ex = null)
	{
		var coloredMessage = GetColoredMessage(message);
		textWriter.Write(coloredMessage);

		if (ex != null)
			textWriter.Write(JsonConvert.SerializeObject(ex, Formatting.Indented));

		textWriter.WriteLine();
	}


	private static string GetColoredMessage<TState>(TState message)
	{
		var messageText = message?.ToString() ?? string.Empty;

		return Regex.Replace(messageText, @"\[([^\]]+)\]", "\x1b[96m[$1]\x1b[0m");
	}

}
