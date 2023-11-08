using DocumentService.Pdf.Models;
using System.ComponentModel.DataAnnotations;

namespace DocumentServiceWebAPI.Models;

public class PdfGenerationRequestDTO
{
    [Required]
    public string? Base64 { get; set; }
    [Required]
    public DocumentData? DocumentData { get; set; }
}
