namespace Movie2Image.Process.Domain.ValueObjects;

public sealed record ZipPath
{
	public string Value { get; }

	private ZipPath(string value)
	{
		Value = value;
	}

	public static ZipPath Create(string? value)
	{
		if (string.IsNullOrWhiteSpace(value))
			throw new ArgumentException("Zip path cannot be null or empty.", nameof(value));

		return new ZipPath(value);
	}

	public static implicit operator string(ZipPath zipPath) => zipPath.Value;
}
