using Microsoft.Extensions.Logging;
using Movie2Image.Process.Application.Ports.Input.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.Text;

namespace Movie2Image.Process.Queue.RabbitMQ;

public class TransformBody(
	ILogger<TransformBody> logger) : ITransformBody
{

	private readonly ReadOnlyMemory<byte> defaultReturn = Array.Empty<byte>();

	public ReadOnlyMemory<byte> GetBytes(object? message)
	{
		if (message == null)
			return defaultReturn;

		var json = JsonConvert.SerializeObject(message);

		try
		{
			JToken.Parse(json!); // Validates JSON format
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "invalid json generated");
		}

		return json != null
			? Encoding.UTF8.GetBytes(json)
			: defaultReturn;
	}

	public object? GetObject(ReadOnlyMemory<byte> message, Type type, bool unzip = false)
	{
		try
		{
			var json = unzip
				? GetZippedJson(message)
				: Encoding.UTF8.GetString(message.ToArray());

			return !string.IsNullOrEmpty(json)
				? JsonConvert.DeserializeObject(json, type)
				: null;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Invalid object data");

			return null;
		}
	}


	private string? GetZippedJson(ReadOnlyMemory<byte> message)
	{
		using var compressedStream = new MemoryStream(message.ToArray());
		using var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
		using var reader = new StreamReader(gzipStream, Encoding.UTF8);

		return reader.ReadToEnd();
	}

}
