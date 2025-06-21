using OsmoDoc.Word.Models;
using System.ComponentModel.DataAnnotations;

namespace OsmoDoc.API.Models;


public class WordGenerationRequestDTO
{
    [Required(ErrorMessage = "Base64 string for Word template is required")]
    public required string Base64 { get; set; }
    [Required(ErrorMessage = "Data to be modified in Word file is required")]
    public WordDocumentDataRequestDTO DocumentData { get; set; } = new();
}

public class WordDocumentDataRequestDTO
{
    public List<ContentData> Placeholders { get; set; } = new List<ContentData>();
    public List<TableData> TablesData { get; set; } = new List<TableData>();
    public List<ImageData> ImagesData { get; set; } = new List<ImageData>();
}
