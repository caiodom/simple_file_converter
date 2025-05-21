using SimpleFileConverter.API.Models.DTOs.Request;
using SimpleFileConverter.API.Models.DTOs.Response;


namespace SimpleFileConverter.API.Interfaces.Services;

public interface IConverterService
{
    Task<ConvertFileResult> ConvertAudioAsync(ConvertAudioRequest request);
    Task<ConvertFileResult> ConvertImageAsync(ConvertImageRequest request);
    Task<ConvertFileResult> ConvertVideoAsync(ConvertVideoRequest request);
    Task<ConvertFileResult> ConvertDocumentAsync(ConvertDocumentRequest request);
}
