using Microsoft.AspNetCore.Mvc;
using SimpleFileConverter.API.Enums.Formats;
using SimpleFileConverter.API.Interfaces.Services;
using SimpleFileConverter.API.Models.DTOs.Request;

namespace SimpleFileConverter.API.Controllers;

[ApiController]
[Route("api/audio")]
public class AudioController(IConverterService converterService) : ControllerBase
{
    [HttpPost("convert")]
    public async Task<IActionResult> Convert(
        [FromForm] IFormFile file,
        [FromForm] AudioFormat inputFormat,
        [FromForm] AudioFormat outputFormat)
    {

        using var fileStream = file.OpenReadStream();

        var request = new ConvertAudioRequest()
        {
            FileStream = fileStream,
            OriginalFileName = file.FileName,
            OutputFormat = outputFormat
        };

        var convertResult = await converterService.ConvertAudioAsync(request);

        if(convertResult.Data == null)
            return BadRequest("Audio conversion failed.");

        return File(convertResult.Data, "audio/*", Path.GetFileName(convertResult.OutputPath));
    }
}
