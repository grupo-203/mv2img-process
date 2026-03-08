using Bogus;

namespace Movie2Image.Process.Application.Test;

public abstract class TestBase
{

	protected Faker Faker { get; } = new();


	protected string GetTempFile()
	{
		var tempFile = Path.GetTempFileName();

		File.WriteAllText(tempFile, Faker.Lorem.Paragraph());

		return tempFile;
	}

}
