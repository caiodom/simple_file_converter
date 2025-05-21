using Microsoft.AspNetCore.Mvc;
using SimpleFileConverter.API.Enums.Formats;
using SimpleFileConverter.API.Interfaces.Services;
using SimpleFileConverter.API.Models.DTOs.Request;

namespace SimpleFileConverter.API.Controllers;

[ApiController]
[Route("api/documents")]
public class DocumentsController(IConverterService converterService) : ControllerBase
{
    [HttpPost("convert")]
    public async Task<IActionResult> Convert(
        [FromForm] IFormFile file,
        [FromForm] DocumentFormat inputFormat,
        [FromForm] DocumentFormat outputFormat)
    {

        ValidateConversion(inputFormat, outputFormat);

        using var fileStream = file.OpenReadStream();
        var request = new ConvertDocumentRequest()
        {
            FileStream = fileStream,
            OriginalFileName = file.FileName,
            InputFormat = inputFormat,
            OutputFormat = outputFormat
        };

        var convertResult = await converterService.ConvertDocumentAsync(request);

        if (convertResult.Data == null)
            return BadRequest("Document conversion failed.");

        var contentType = HandleContentType(outputFormat);
        return File(convertResult.Data, contentType, Path.GetFileName(convertResult.OutputPath));
    }


    private string HandleContentType(DocumentFormat outputFormat)
    {

        var contentType = outputFormat switch
        {
            DocumentFormat.Pdf => "application/pdf",
            DocumentFormat.Html => "text/html",
            DocumentFormat.Markdown => "text/markdown",
            _ => "application/octet-stream"
        };

        return contentType;
    }

    private void ValidateConversion(DocumentFormat inputFormat, DocumentFormat outputFormat)
    {
        if (inputFormat == DocumentFormat.Pdf && outputFormat != DocumentFormat.Html)
            throw new InvalidOperationException($"Conversion from PDF to {outputFormat} is not supported.");
        if (outputFormat != DocumentFormat.Pdf
            && outputFormat != DocumentFormat.Html)
            throw new InvalidOperationException($"Output format {outputFormat} is not supported.");
    }
}