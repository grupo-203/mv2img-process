using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Services;
using Movie2Image.Process.Domain.Exceptions;
using System.Collections.Generic;

namespace Movie2Image.Process.Application.Test.Services;

public class ZipPathSetServiceTest : TestBase
{

	[Fact]
	public void ZipPathSetServiceTest_Ok()
	{
		// Arrange
		var conf = GetConfiguration("/foo");
		var procConfig = new TestProcessConfiguration(conf);
		var data  = new ProcessMovieDto
		{
			TempZipPath = Faker.System.FilePath().TrimEnd('/')
		};
		var file = Path.GetFileName(data.TempZipPath);
		var service = new ZipPathSetService(procConfig);

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
		var conf = GetConfiguration();
		var procConfig = new TestProcessConfiguration(conf);
		var data = new ProcessMovieDto
		{
			TempZipPath = Faker.System.FilePath().TrimEnd('/')
		};
		var file = Path.GetFileName(data.TempZipPath);
		var service = new ZipPathSetService(procConfig);

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
		var conf = GetConfiguration();
		var procConfig = new TestProcessConfiguration(conf);
		var data = new ProcessMovieDto
		{
			TempZipPath = value
		};
		var service = new ZipPathSetService(procConfig);

		// Act
		var act = () => service.Set(data);

		// Assert
		act.Should().Throw<ValidationException>();
	}


	private IConfiguration GetConfiguration(string? zipPath = null)
	{
		var builder = new ConfigurationBuilder();

		// Provide FRAMES_PATH default because TestProcessConfiguration expects it
		var dict = new Dictionary<string, string?>
		{
			{ "FRAMES_PATH", "/tmp/frames" }
		};

		if (!string.IsNullOrEmpty(zipPath))
		{
			dict["ZIP_PATH"] = zipPath;
		}

		builder.AddInMemoryCollection(dict);

		return builder.Build();
	}

}
