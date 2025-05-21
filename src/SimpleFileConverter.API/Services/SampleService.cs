using SimpleFileConverter.API.Enums.Formats;
using SimpleFileConverter.API.Interfaces.Services;
using SimpleFileConverter.API.Models.DTOs.Response;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace SimpleFileConverter.API.Services;

public class SampleService: ISampleService
{
    private readonly IWebHostEnvironment _env;
    private const int MaxDimension = 5000;
    private const int MinDimension = 1;
    private const string SamplesFolder = "samples";

    public SampleService(IWebHostEnvironment env)
    {
        _env = env ?? throw new ArgumentNullException(nameof(env));
    }

    public SampleDocumentResult GetDocumentSample(DocumentFormat format)
    {
        var (fileName, contentType) = GetDocumentFileNameAndContentType(format);

        if (fileName is null)
            return ErrorDocumentResult("Document format not supported.");

        var path = Path.Combine(_env.WebRootPath!, SamplesFolder, fileName);
        if (!File.Exists(path))
            return ErrorDocumentResult($"File '{fileName}' not found.");

        return new SampleDocumentResult
        {
            FilePath = path,
            ContentType = contentType,
            FileName = fileName
        };
    }


    public async Task<SampleFileResult> GenerateImageSample(int width, int height, ImageFormat format)
    {
        if (!IsValidDimension(width) || !IsValidDimension(height))
            return ErrorFileResult("Width and height must be between 1 and 5000.");

        using var img = CreateRandomImage(width, height);
        using var ms = new MemoryStream();

        var (encoder, contentType, ext) = GetImageEncoderAndContentType(format);
        if (encoder is null)
            return ErrorFileResult("Unsupported image format.");

        await img.SaveAsync(ms, encoder);

        return new SampleFileResult
        {
            Data = ms.ToArray(),
            ContentType = contentType,
            FileName = $"sample.{ext}"
        };
    }

    public async Task<SampleFileResult> GenerateVideoSample(VideoFormat format, int duration, string resolution)
    {
        var ext = format.ToString().ToLowerInvariant();
        var tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + $".{ext}");

        var ffmpegArgs = GetFfmpegVideoArgs(format, duration, resolution, tempFile);
        if (ffmpegArgs is null)
            return ErrorFileResult("Format not supported for video sample.");

        var ok = await ConverterHelper.RunCli("ffmpeg", ffmpegArgs);
        if (!ok)
            return ErrorFileResult("Failed to generate video sample.");

        var data = await File.ReadAllBytesAsync(tempFile);
        var contentType = GetVideoContentType(format);

        return new SampleFileResult
        {
            Data = data,
            ContentType = contentType,
            FileName = $"sample.{ext}"
        };
    }

    public async Task<SampleFileResult> GenerateAudioSample(AudioFormat format, int duration)
    {
        var ext = format.ToString().ToLowerInvariant();
        var tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + $".{ext}");

        var ffmpegArgs = GetFfmpegAudioArgs(format, duration, tempFile);
        if (ffmpegArgs is null)
            return ErrorFileResult("Format not supported for audio sample.");

        var ok = await ConverterHelper.RunCli("ffmpeg", ffmpegArgs);
        if (!ok)
            return ErrorFileResult("Failed to generate audio sample.");

        var data = await File.ReadAllBytesAsync(tempFile);
        var contentType = GetAudioContentType(format);

        return new SampleFileResult
        {
            Data = data,
            ContentType = contentType,
            FileName = $"sample.{ext}"
        };
    }

    #region Private Helpers

    private static bool IsValidDimension(int value) => value >= MinDimension && value <= MaxDimension;

    private static (string? fileName, string contentType) GetDocumentFileNameAndContentType(DocumentFormat format) =>
        format switch
        {
            DocumentFormat.Pdf => ("sample.pdf", "application/pdf"),
            DocumentFormat.Html => ("sample.html", "text/html"),
            DocumentFormat.Markdown => ("sample.md", "text/markdown"),
            _ => (null, "application/octet-stream")
        };

    private static (IImageEncoder? encoder, string contentType, string ext) GetImageEncoderAndContentType(ImageFormat format) =>
        format switch
        {
            ImageFormat.Png => (new PngEncoder(), "image/png", "png"),
            ImageFormat.Jpg => (new JpegEncoder(), "image/jpeg", "jpg"),
            ImageFormat.Gif => (new GifEncoder(), "image/gif", "gif"),
            _ => (null, string.Empty, string.Empty)
        };

    private static string? GetFfmpegVideoArgs(VideoFormat format, int duration, string resolution, string tempFile) =>
        format switch
        {
            VideoFormat.Webm =>
                $"-f lavfi -i smptebars=size={resolution}:rate=25 -t {duration} -c:v libvpx -b:v 1M {tempFile}",
            VideoFormat.Mp4 or VideoFormat.Mov or VideoFormat.Mkv or VideoFormat.Avi
            or VideoFormat.Flv or VideoFormat.Mpeg or VideoFormat.Wmv =>
                $"-f lavfi -i smptebars=size={resolution}:rate=25 -t {duration} -c:v libx264 -pix_fmt yuv420p {tempFile}",
            _ => null
        };

    private static string? GetFfmpegAudioArgs(AudioFormat format, int duration, string tempFile) =>
        format switch
        {
            AudioFormat.Wav =>
                $"-f lavfi -i sine=frequency=1000:duration={duration} -c:a pcm_s16le {tempFile}",
            AudioFormat.Mp3 =>
                $"-f lavfi -i sine=frequency=1000:duration={duration} -c:a libmp3lame -qscale:a 2 {tempFile}",
            _ => null
        };

    private static string GetVideoContentType(VideoFormat format) =>
        format switch
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

    private static string GetAudioContentType(AudioFormat format) =>
        format switch
        {
            AudioFormat.Mp3 => "audio/mpeg",
            AudioFormat.Wav => "audio/wav",
            _ => "application/octet-stream"
        };

    private static Image<Rgba32> CreateRandomImage(int width, int height)
    {
        var rnd = new Random();
        var img = new Image<Rgba32>(width, height);
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
        return img;
    }

    private static SampleDocumentResult ErrorDocumentResult(string error) =>
        new() { Error = error };

    private static SampleFileResult ErrorFileResult(string error) =>
        new() { Error = error };

    #endregion
}
