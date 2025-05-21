using SimpleFileConverter.API.Enums.Formats;
using SimpleFileConverter.API.Models.DTOs.Request.Base;

namespace SimpleFileConverter.API.Models.DTOs.Request;

public class ConvertAudioRequest:ConvertRequest
{
    public AudioFormat OutputFormat { get; set; }
}
