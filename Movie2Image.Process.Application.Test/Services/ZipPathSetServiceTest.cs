using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Services;
using Movie2Image.Process.Domain.Exceptions;

namespace Movie2Image.Process.Application.Test.Services;

public class ZipPathSetServiceTest : TestBase
{

	[Fact]
	public void ZipPathSetServiceTest_Ok()
	{
		// Arrange
		var config = GetConfiguration("/foo");
		var data  = new ProcessMovieDto
		{
			TempZipPath = Faker.System.FilePath().TrimEnd('/')
		};
		var file = Path.GetFileName(data.TempZipPath);
		var service = new ZipPathSetService(config);

		// Act
		service.Set(data);

		// Assert
		data.Should().NotBeNull();
		data.ZipPath.Should().Be($"/foo/{file}");
	}

	[Fact]
	public void ZipPathSetServiceTest_No_ZipPath_Ok()
	{
		// Arrange
		var config = GetConfiguration();
		var data = new ProcessMovieDto
		{
			TempZipPath = Faker.System.FilePath().TrimEnd('/')
		};
		var file = Path.GetFileName(data.TempZipPath);
		var service = new ZipPathSetService(config);

		// Act
		service.Set(data);

		// Assert
		data.Should().NotBeNull();
		data.ZipPath.Should().Be($"/home/zip_path/{file}");
	}

	[Theory]
	[InlineData(null)]
	[InlineData(" ")]
	[InlineData("")]
	public void ZipPathSetServiceTest_Invalid_TempZipPath(string? value)
	{
		// Arrange
		var config = GetConfiguration();
		var data = new ProcessMovieDto
		{
			TempZipPath = value
		};
		var service = new ZipPathSetService(config);

		// Act
		var act = () => service.Set(data);

		// Assert
		act.Should().Throw<ValidationException>();
	}


	private IConfiguration GetConfiguration(string? zipPath = null)
	{
		var builder = new ConfigurationBuilder();

		if (!string.IsNullOrEmpty(zipPath))
		{
			builder.AddInMemoryCollection(new Dictionary<string, string>
				{{ "ZIP_PATH", zipPath }}!);
		}

		return builder.Build();
	}

}
