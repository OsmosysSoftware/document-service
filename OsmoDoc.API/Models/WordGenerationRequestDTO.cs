using OsmoDoc.Word.Models;
using System.ComponentModel.DataAnnotations;

namespace OsmoDoc.API.Models;


public class WordGenerationRequestDTO
{
    [Required(ErrorMessage = "Base64 string for Word template is required")]
    public string? Base64 { get; set; }
    [Required(ErrorMessage = "Data to be modified in Word file is required")]
    public WordDocumentDataRequestDTO? DocumentData { get; set; }
}

public class WordContentDataRequestDTO : ContentData
{
    public string? ImageExtension { get; set; }
}

public class WordDocumentDataRequestDTO
{
    public List<WordContentDataRequestDTO> Placeholders { get; set; } = new List<WordContentDataRequestDTO>();
    public List<TableData> TablesData { get; set; } = new List<TableData>();
}
