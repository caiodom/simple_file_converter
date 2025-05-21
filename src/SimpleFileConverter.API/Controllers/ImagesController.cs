using Microsoft.AspNetCore.Mvc;
using SimpleFileConverter.API.Enums.Formats;
using SimpleFileConverter.API.Interfaces.Services;
using SimpleFileConverter.API.Models.DTOs.Request;

namespace SimpleFileConverter.API.Controllers
{
    [ApiController]
    [Route("api/images")]
    public class ImagesController(IConverterService converterService) : ControllerBase
    {
        [HttpPost("convert")]
        public async Task<IActionResult> Convert(
            [FromForm] IFormFile file,
            [FromForm] ImageFormat inputFormat,
            [FromForm] ImageFormat outputFormat)
        {
            using var fileStream = file.OpenReadStream();
            var request = new ConvertImageRequest()
            {
                FileStream = fileStream,
                OriginalFileName = file.FileName,
                OutputFormat = outputFormat
            };

            var convertResult = await converterService.ConvertImageAsync(request);
            if (convertResult.Data == null)
                return BadRequest("Image conversion failed.");

            var contentType=HandleContentType(outputFormat);

            return File(convertResult.Data, contentType, Path.GetFileName(convertResult.OutputPath));
        }


        private string HandleContentType(ImageFormat outputFormat)
        {
            var contentType = outputFormat switch
            {
                ImageFormat.Png => "image/png",
                ImageFormat.Jpg => "image/jpeg",
                _ => "application/octet-stream"
            };

            return contentType;
        }
    }
}
