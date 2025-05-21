using SimpleFileConverter.API.Enums.Formats;
using SimpleFileConverter.API.Interfaces.Services;

namespace SimpleFileConverter.API.Services.Converters
{
    public class ImageConverter : IConverter
    {
        public Task<bool> ConvertAsync(string inputPath, string outputPath)
            => ConverterHelper.RunCli("magick", $"{inputPath} {outputPath}");
    }
}
