using DocumentService.Pdf.Models;
using System.ComponentModel.DataAnnotations;

namespace DocumentService.API.Models;

public class PdfGenerationRequestDTO
{
    [Required(ErrorMessage = "Base64 string for PDF template is required")]
    public string? Base64 { get; set; }
    [Required(ErrorMessage = "Data to be modified in PDF is required")]
    public DocumentData? DocumentData { get; set; }
}
