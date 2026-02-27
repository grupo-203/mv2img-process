namespace Movie2Image.Process.Application.Ports.Output.Media;

public interface IMovieIntoImagesTransform
{


	Task Transform(string moviePath, string framesPath);

}
