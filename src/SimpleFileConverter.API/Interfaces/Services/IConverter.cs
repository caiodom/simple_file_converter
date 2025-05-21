namespace SimpleFileConverter.API.Interfaces.Services;

public interface IConverter
{
    Task<bool> ConvertAsync(string inputPath, string outputPath);
}
