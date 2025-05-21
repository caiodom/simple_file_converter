using SimpleFileConverter.API.Enums.Formats;
using SimpleFileConverter.API.Interfaces.Services;

namespace SimpleFileConverter.API.Services.Converters;

public class DocumentConverter : IConverter
{
    private readonly DocumentFormat _input;
    private readonly DocumentFormat _output;

    public DocumentConverter(DocumentFormat input, DocumentFormat output)
    {
        _input = input;
        _output = output;
    }

    public Task<bool> ConvertAsync(string inputPath, string outputPath)
    {
        string? args = _output switch
        {
            DocumentFormat.Pdf =>
                $"--headless --convert-to pdf --outdir {Path.GetDirectoryName(outputPath)} {inputPath}",
            DocumentFormat.Html =>
                $"--headless --convert-to html --outdir {Path.GetDirectoryName(outputPath)} {inputPath}",
            DocumentFormat.Markdown =>
                $"-f {_input.ToString().ToLower()} -t markdown -o {outputPath} {inputPath}",
            _ => null
        };
        if (args is null) return Task.FromResult(false);
        var exe = _output == DocumentFormat.Markdown ? "pandoc" : "soffice";
        return ConverterHelper.RunCli(exe, args);
    }
}
