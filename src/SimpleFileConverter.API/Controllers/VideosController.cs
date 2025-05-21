using Microsoft.AspNetCore.Mvc;
using SimpleFileConverter.API.Enums.Formats;
using SimpleFileConverter.API.Interfaces.Services;
using SimpleFileConverter.API.Models.DTOs.Request;

namespace SimpleFileConverter.API.Controllers;

[ApiController]
[Route("api/videos")]
public class VideosController(IConverterService converterService) : ControllerBase
{
    [HttpPost("convert")]
    public async Task<IActionResult> Convert(
        [FromForm] IFormFile file,
        [FromForm] VideoFormat inputFormat,
        [FromForm] VideoFormat outputFormat)
    {
        using var fileStream = file.OpenReadStream();
        var request = new ConvertVideoRequest()
        {
            FileStream = fileStream,
            OriginalFileName = file.FileName,
            OutputFormat = outputFormat
        };

        var convertResult = await converterService.ConvertVideoAsync(request);

        if (convertResult.Data == null)
            return BadRequest("Video conversion failed.");

        return File(convertResult.Data, "video/*", Path.GetFileName(convertResult.OutputPath));
    }
}
