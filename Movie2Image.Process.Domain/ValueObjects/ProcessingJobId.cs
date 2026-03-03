namespace Movie2Image.Process.Domain.ValueObjects;

public sealed record ProcessingJobId
{
	public string Value { get; }

	private ProcessingJobId(string value)
	{
		Value = value;
	}

	public static ProcessingJobId Create(string? value)
	{
		if (string.IsNullOrWhiteSpace(value))
			throw new ArgumentException("Processing job ID cannot be null or empty.", nameof(value));

		return new ProcessingJobId(value);
	}

	public static ProcessingJobId From(Guid guid) => new(guid.ToString());

	public static implicit operator string(ProcessingJobId id) => id.Value;
}
