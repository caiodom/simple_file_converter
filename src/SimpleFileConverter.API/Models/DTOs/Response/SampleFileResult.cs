using SimpleFileConverter.API.Models.DTOs.Response.Base;

namespace SimpleFileConverter.API.Models.DTOs.Response;

public class SampleFileResult:SampleResult
{
    public byte[]? Data { get; set; }
}
