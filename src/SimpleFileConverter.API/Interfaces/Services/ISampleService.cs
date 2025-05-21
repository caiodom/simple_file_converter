using SimpleFileConverter.API.Enums.Formats;
using SimpleFileConverter.API.Models.DTOs.Response;

namespace SimpleFileConverter.API.Interfaces.Services;

public interface ISampleService
{
    Task<SampleFileResult> GenerateImageSample(int width, int height, ImageFormat format);
    Task<SampleFileResult> GenerateVideoSample(VideoFormat format, int duration, string resolution);
    Task<SampleFileResult> GenerateAudioSample(AudioFormat format, int duration);
}
