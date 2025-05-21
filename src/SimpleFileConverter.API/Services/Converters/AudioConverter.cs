using SimpleFileConverter.API.Enums.Formats;
using SimpleFileConverter.API.Interfaces.Services;

namespace SimpleFileConverter.API.Services.Converters;

public class AudioConverter : IConverter
{
    public Task<bool> ConvertAsync(string inputPath, string outputPath)
        => ConverterHelper.RunCli("ffmpeg", $"-i {inputPath} {outputPath}");
}
