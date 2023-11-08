using System.ComponentModel.DataAnnotations;
using DocumentService.Word.Models;
namespace DocumentServiceWebAPI.Models;


public class WordGenerationRequestDTO
{
    [Required]
    public string Base64 { get; set; }
    [Required]
    public DocumentData DocumentData { get; set; }
}
