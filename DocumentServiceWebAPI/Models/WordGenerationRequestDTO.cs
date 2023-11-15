using DocumentService.Word.Models;
using System.ComponentModel.DataAnnotations;
namespace DocumentServiceWebAPI.Models;


public class WordGenerationRequestDTO
{
    [Required(ErrorMessage = "Base64 string for Word template is required")]
    public string? Base64 { get; set; }
    [Required(ErrorMessage = "Data to be modified in Word file is required")]
    public DocumentData? DocumentData { get; set; }
}
