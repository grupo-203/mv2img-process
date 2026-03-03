using Bogus;

namespace Movie2Image.Process.Domain.Test;

public abstract class TestBase
{

	protected Faker Faker { get; } = new();

}
