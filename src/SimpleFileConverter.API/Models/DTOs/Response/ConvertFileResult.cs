namespace SimpleFileConverter.API.Models.DTOs.Response;

public class ConvertFileResult
{
    public byte[]? Data { get; set; }
    public string OutputPath { get; set; }
}
