using DocumentServiceWebAPI.Models;
using DocumentService.Word;
using Microsoft.AspNetCore.Mvc;
using DocumentServiceWebAPI.Helpers;

namespace DocumentServiceWebAPI.Controllers;
[Route("api")]
[ApiController]
public class WordController : ControllerBase
{
    [HttpPost]
    [Route("word/GenerateWordDocument")]
    public IActionResult GenerateWord(WordGenerationRequestDTO request)
    {
        try
        {
            string templateFilePath = Base64Helper.ConvertBase64ToFile(request.Base64, "word");
            DocumentService.Word.Models.DocumentData documentData = new()
            {
                Placeholders = request.DocumentData.Placeholders,
                TablesData = request.DocumentData.TablesData
            };
            // Generate the doc using the word template
            string outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "convertedDocs");
            // Creating a directory for the converted output PDFs if it doesn't exist
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            string outputFilePath = Path.Combine(outputFolder, "wordOutput.docx");

            WordDocumentGenerator.GenerateDocumentByTemplate(templateFilePath, documentData, outputFilePath);
            string base64 = Base64Helper.ConvertFileToBase64(outputFilePath);

            return Ok(new { message = "Word document generated successfully.", Base64 = base64 });
        }
        catch (Exception ex)
        {
            return BadRequest($"Word document generation failed: {ex.Message}");
        }
    }
}
