namespace Movie2Image.Process.Domain.Exceptions;

public class MaxRetriesExceededException : Exception
{
	public MaxRetriesExceededException(string message) : base(message)
	{
	}
}
