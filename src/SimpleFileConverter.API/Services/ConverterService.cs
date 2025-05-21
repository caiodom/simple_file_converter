using SimpleFileConverter.API.Interfaces.Services;
using SimpleFileConverter.API.Models.DTOs.Request;
using SimpleFileConverter.API.Models.DTOs.Response;

namespace SimpleFileConverter.API.Services;

public class ConverterService : IConverterService
{
    public async Task<ConvertFileResult> ConvertAudioAsync(ConvertAudioRequest request)
    {
        var converter = ConverterHandler
                             .CreateAudioConverter();

        var processRequest = new ProcessFileRequest(converter, 
                                                    request.OriginalFileName, 
                                                    request.FileStream, 
                                                    request.OutputFormat.ToString());

        return await ProcessFileAsync(processRequest);
    }

    public async Task<ConvertFileResult> ConvertImageAsync(ConvertImageRequest request)
    {
        var converter = ConverterHandler
                            .CreateImageConverter();

        var processRequest = new ProcessFileRequest(converter,
                                                    request.OriginalFileName,
                                                    request.FileStream,
                                                    request.OutputFormat.ToString());

        return await ProcessFileAsync(processRequest);
    }

    public async Task<ConvertFileResult> ConvertVideoAsync(ConvertVideoRequest request)
    {
        var converter = ConverterHandler
                            .CreateVideoConverter();

        var processRequest = new ProcessFileRequest(converter,
                                                    request.OriginalFileName,
                                                    request.FileStream,
                                                    request.OutputFormat.ToString());

        return await ProcessFileAsync(processRequest);
    }

    public async Task<ConvertFileResult> ConvertDocumentAsync(ConvertDocumentRequest request)
    {

        var converter = ConverterHandler
                            .CreateDocumentConverter(request.InputFormat, request.OutputFormat);

        var processRequest = new ProcessFileRequest(converter,
                                                    request.OriginalFileName,
                                                    request.FileStream,
                                                    request.OutputFormat.ToString());

        return await ProcessFileAsync(processRequest);
    }


    private async Task<ConvertFileResult> ProcessFileAsync(ProcessFileRequest processFileRequest)
    {
        var inputOutputPath = await SaveDocumentsAsync(processFileRequest.fileStream,
                                                       processFileRequest.originalFileName,
                                                       processFileRequest.outputFormat);

        var outputPath = inputOutputPath.outputPath;
        var success = await processFileRequest.converter.ConvertAsync(inputOutputPath.inputPath, outputPath);
        if (!success)
            throw new InvalidOperationException("Conversion failed.");

        var data = await File.ReadAllBytesAsync(outputPath);
        var result = new ConvertFileResult { Data = data, OutputPath = outputPath };

        return result;
    }
    private async Task<SaveDocumentResult> SaveDocumentsAsync(Stream fileStream, string originalFileName, string outputFormat)
    {
        var temp = Path.GetTempPath() + Path.GetRandomFileName();
        var inputPath = temp + Path.GetExtension(originalFileName);
        await using (var inStream = File.Create(inputPath))
            await fileStream.CopyToAsync(inStream);

        var outputPath = temp + "." + outputFormat.ToLower();
        return new SaveDocumentResult(inputPath, outputPath);
    }
    public record SaveDocumentResult(string inputPath, string outputPath);
    public record ProcessFileRequest(IConverter converter, string originalFileName, Stream fileStream, string outputFormat);


}
