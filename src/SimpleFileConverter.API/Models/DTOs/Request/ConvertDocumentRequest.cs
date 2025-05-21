using SimpleFileConverter.API.Enums.Formats;
using SimpleFileConverter.API.Models.DTOs.Request.Base;

namespace SimpleFileConverter.API.Models.DTOs.Request;

public class ConvertDocumentRequest:ConvertRequest
{
    public DocumentFormat InputFormat { get; set; }
    public DocumentFormat OutputFormat { get; set; }
}
