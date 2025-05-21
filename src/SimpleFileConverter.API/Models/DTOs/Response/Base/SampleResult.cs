namespace SimpleFileConverter.API.Models.DTOs.Response.Base
{
    public abstract class SampleResult
    {
        public string?  ContentType { get; set; }
        public string? FileName { get; set; }
        public string? Error { get; set; }

    }
}
