namespace Movie2Image.Process.Domain.ValueObjects;

public sealed record FramesPath
{
	public string Value { get; }

	private FramesPath(string value)
	{
		Value = value;
	}

	public static FramesPath Create(string? value)
	{
		if (string.IsNullOrWhiteSpace(value))
			throw new ArgumentException("Frames path cannot be null or empty.", nameof(value));

		return new FramesPath(value);
	}

	public static implicit operator string(FramesPath framesPath) => framesPath.Value;
}
