using Movie2Image.Process.Application.Ports.Input.Queue;

namespace Movie2Image.Process.Queue.Common;

public class QueueNames : IQueueNames
{

    public string ProcessMovie => "carregamentos.video.request";
    
    public string ExtractFrames => "extract-frames";
    
    public string GenerateZip => "generate-zip";

    public string Publish => "publish";

	public string DeadLetter => "dead-letter";

    public string Finished => "carregamentos.video.finished";

    public string Error => "carregamentos.video.error";

}
