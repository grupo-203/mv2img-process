namespace Movie2Image.Process.Domain.ValueObjects;

public sealed record UserId
{
	public string Value { get; }

	private UserId(string value)
	{
		Value = value;
	}

	public static UserId Create(string? value)
	{
		if (string.IsNullOrWhiteSpace(value))
			throw new ArgumentException("User ID cannot be null or empty.", nameof(value));

		return new UserId(value);
	}

	public static UserId From(Guid guid) => new(guid.ToString());

	public static implicit operator string(UserId userId) => userId.Value;
}
