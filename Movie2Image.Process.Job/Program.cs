using Microsoft.Extensions.DependencyInjection;
using Movie2Image.Process.Application;
using Movie2Image.Process.Application.Ports.Input.Controller;
using Movie2Image.Process.Application.Ports.Input.Queue;
using Movie2Image.Process.Controller;
using Movie2Image.Process.ExternalServices;
using Movie2Image.Process.Instrumentalization;
using Movie2Image.Process.Job;
using Movie2Image.Process.Logging;
using Movie2Image.Process.Mail;
using Movie2Image.Process.Media;
using Movie2Image.Process.Queue;
using Movie2Image.Process.Storage;
using Movie2Image.Process.Zip;

var version = new Version(1, 0, 0);

ConsoleLogger.LogInformation("Start [Movie2Image.Process.Job]");
ConsoleLogger.LogInformation($"Version [{version}]");
ConsoleLogger.LogInformation("Adding services");

var services = new ServiceCollection()
	.AddConfiguration()
	.AddMemoryCache()
	.AddConsoleLogging()
	.AddInstrumentalization()
	.AddMedia()
	.AddZip()
	.AddStorage()
	.AddMail()
	.AddQueues()
	.AddExternalServices()
	.AddApplication()
	.AddControllers()
	.BuildServiceProvider();

var queues = services.GetRequiredService<IQueueNames>();

await services.GetRequiredService<IQueueList>()
	.Add<IDeadLetterController>(queues.DeadLetter)
	.Add<IPublishController>(queues.Publish)
	.Add<IGenerateZipController>(queues.GenerateZip)
	.Add<IExtractFramesController>(queues.ExtractFrames)
	.Add<IProcessMovieController>(queues.ProcessMovie)
	.Start();
