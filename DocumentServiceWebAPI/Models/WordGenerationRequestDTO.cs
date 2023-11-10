using DocumentService.Word.Models;
using System.ComponentModel.DataAnnotations;
namespace DocumentServiceWebAPI.Models;


public class WordGenerationRequestDTO
{
    [Required]
    public string? Base64 { get; set; }
    [Required]
    public DocumentData? DocumentData { get; set; }
}
