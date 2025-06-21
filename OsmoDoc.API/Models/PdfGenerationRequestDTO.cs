using OsmoDoc.Pdf.Models;
using System.ComponentModel.DataAnnotations;

namespace OsmoDoc.API.Models;

public class PdfGenerationRequestDTO
{
    [Required(ErrorMessage = "Base64 string for PDF template is required")]
    public required string Base64 { get; set; }
    public DocumentData DocumentData { get; set; } = new();
    public string? SerializedEjsDataJson { get; set; }
}
