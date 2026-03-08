namespace Movie2Image.Process.Domain.Enums;

public enum ProcessingStatus
{
	Created,
	ExtractingFrames,
	FramesExtracted,
	Compressing,
	Compressed,
	Completed,
	Failed
}
