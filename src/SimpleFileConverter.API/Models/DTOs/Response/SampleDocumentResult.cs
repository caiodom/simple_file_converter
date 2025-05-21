using SimpleFileConverter.API.Models.DTOs.Response.Base;

namespace SimpleFileConverter.API.Models.DTOs.Response
{
    public class SampleDocumentResult:SampleResult
    {
        public string? FilePath { get; set; }
    }
}
