using Microsoft.AspNetCore.Mvc;
using SimpleFileConverter.API.Enums.Formats;
using SimpleFileConverter.API.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace SimpleFileConverter.API.Controllers
{
    [ApiController]
    [Route("api/samples")]
    public class SamplesController(IWebHostEnvironment env) : ControllerBase
    {
        [HttpGet("document")]
        public IActionResult DocumentSample([FromQuery] DocumentFormat format = DocumentFormat.Pdf)
        {
            // Define o nome do arquivo e o content-type
            string fileName = format switch
            {
                DocumentFormat.Pdf => "sample.pdf",
                DocumentFormat.Html => "sample.html",
                DocumentFormat.Markdown => "sample.md",
                _ => null
            };
            if (fileName == null)
                return BadRequest("Formato de documento não suportado.");

            string contentType = format switch
            {
                DocumentFormat.Pdf => "application/pdf",
                DocumentFormat.Html => "text/html",
                DocumentFormat.Markdown => "text/markdown",
                _ => "application/octet-stream"
            };

            var path = Path.Combine(env.WebRootPath!, "samples", fileName);
            if (!System.IO.File.Exists(path))
                return NotFound($"Arquivo '{fileName}' não encontrado.");

            // Retorna o arquivo diretamente do disco
            return PhysicalFile(path, contentType, fileName);
        }

        [HttpGet("image")]
        public async Task<IActionResult> ImageSample(
            [FromQuery] int width = 800,
            [FromQuery] int height = 600,
            [FromQuery] ImageFormat format = ImageFormat.Png)
        {
            if (width <= 0 || height <= 0 || width > 5000 || height > 5000)
                return BadRequest("Width and height must be between 1 and 5000.");

            var rnd = new Random();
            using var img = new Image<Rgba32>(width, height);
            img.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < width; x++)
                    {
                        row[x] = new Rgba32(
                            (byte)rnd.Next(256),
                            (byte)rnd.Next(256),
                            (byte)rnd.Next(256));
                    }
                }
            });

            using var ms = new MemoryStream();
            string contentType;

            switch (format)
            {
                case ImageFormat.Png:
                    await img.SaveAsync(ms, new PngEncoder());
                    contentType = "image/png";
                    break;
                case ImageFormat.Jpg:
                    await img.SaveAsync(ms, new JpegEncoder());
                    contentType = "image/jpeg";
                    break;
                case ImageFormat.Gif:
                    await img.SaveAsync(ms, new GifEncoder());
                    contentType = "image/gif";
                    break;
                default:
                    return BadRequest("Unsupported image format.");
            }

            return File(ms.ToArray(), contentType, $"sample.{format.ToString().ToLower()}");
        }


        [HttpGet("video")]
        public async Task<IActionResult> VideoSample(
            [FromQuery] VideoFormat format = VideoFormat.Mp4,
            [FromQuery] int duration = 5,
            [FromQuery] string resolution = "320x240")
        {
            var ext = format.ToString().ToLower();
            var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ext);

            // Monta argumentos do FFmpeg
            string? ffmpegArgs = format switch
            {
                VideoFormat.Webm =>
                    $"-f lavfi -i smptebars=size={resolution}:rate=25 -t {duration} -c:v libvpx -b:v 1M {tempFile}",
                VideoFormat.Mp4 or VideoFormat.Mov or VideoFormat.Mkv or VideoFormat.Avi
                or VideoFormat.Flv or VideoFormat.Mpeg or VideoFormat.Wmv =>
                    $"-f lavfi -i smptebars=size={resolution}:rate=25 -t {duration} -c:v libx264 -pix_fmt yuv420p {tempFile}",
                _ => null
            };

            if (ffmpegArgs == null)
                return BadRequest("Format not supported for video sample.");

            var ok = await ConverterHelper.RunCli("ffmpeg", ffmpegArgs);
            if (!ok)
                return StatusCode(500, "Failed to generate video sample.");

            var data = await System.IO.File.ReadAllBytesAsync(tempFile);

            // Define Content-Type de acordo com o contêiner
            string contentType = format switch
            {
                VideoFormat.Webm => "video/webm",
                VideoFormat.Mp4 => "video/mp4",
                VideoFormat.Mov => "video/quicktime",
                VideoFormat.Mkv => "video/x-matroska",
                VideoFormat.Avi => "video/x-msvideo",
                VideoFormat.Flv => "video/x-flv",
                VideoFormat.Mpeg => "video/mpeg",
                VideoFormat.Wmv => "video/x-ms-wmv",
                _ => "application/octet-stream"
            };

            return File(data, contentType, $"sample.{ext}");
        }

        // 4) Audio sample: onda senoidal em WAV ou MP3
        [HttpGet("audio")]
        public async Task<IActionResult> AudioSample(
            [FromQuery] AudioFormat format = AudioFormat.Wav,
            [FromQuery] int duration = 5)
        {
            var ext = format.ToString().ToLower();
            var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ext);

            string? ffmpegArgs = format switch
            {
                AudioFormat.Wav =>
                    $"-f lavfi -i sine=frequency=1000:duration={duration} -c:a pcm_s16le {tempFile}",
                AudioFormat.Mp3 =>
                    $"-f lavfi -i sine=frequency=1000:duration={duration} -c:a libmp3lame -qscale:a 2 {tempFile}",
                _ => null
            };

            if (ffmpegArgs == null)
                return BadRequest("Format not supported for audio sample.");

            var ok = await ConverterHelper.RunCli("ffmpeg", ffmpegArgs);
            if (!ok)
                return StatusCode(500, "Failed to generate audio sample.");

            var data = await System.IO.File.ReadAllBytesAsync(tempFile);
            var contentType = format == AudioFormat.Mp3 ? "audio/mpeg" : "audio/wav";

            return File(data, contentType, $"sample.{ext}");
        }
    }
}
