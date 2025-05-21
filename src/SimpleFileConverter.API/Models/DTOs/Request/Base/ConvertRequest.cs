namespace SimpleFileConverter.API.Models.DTOs.Request.Base;

public abstract class ConvertRequest
{
    public Stream FileStream { get; set; }
    public string OriginalFileName { get; set; }
}
