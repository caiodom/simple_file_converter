using SimpleFileConverter.API.Interfaces.Services;

namespace SimpleFileConverter.API.Services.Converters;

public class VideoConverter : IConverter
{
    public Task<bool> ConvertAsync(string inputPath, string outputPath)
        => ConverterHelper.RunCli("ffmpeg", $"-i {inputPath} {outputPath}");
}
