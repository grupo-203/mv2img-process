namespace Movie2Image.Process.Domain.ValueObjects;

public sealed record MoviePath
{
	public string Value { get; }

	private MoviePath(string value)
	{
		Value = value;
	}

	public static MoviePath Create(string? value)
	{
		if (string.IsNullOrWhiteSpace(value))
			throw new ArgumentException("Movie path cannot be null or empty.", nameof(value));

		return new MoviePath(value);
	}

	public static implicit operator string(MoviePath moviePath) => moviePath.Value;
}
