using SimpleFileConverter.API.Enums.Formats;
using SimpleFileConverter.API.Interfaces.Services;
using SimpleFileConverter.API.Services.Converters;

namespace SimpleFileConverter.API.Services;

public static class ConverterHandler
{
    public static IConverter CreateDocumentConverter(
        DocumentFormat input, DocumentFormat output)
        => new DocumentConverter(input, output);

    public static IConverter CreateImageConverter()
        => new ImageConverter();

    public static IConverter CreateVideoConverter()
        => new VideoConverter();

    public static IConverter CreateAudioConverter()
        => new AudioConverter();
}
