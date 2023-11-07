using DocumentServiceWebAPI.Models;
using DocumentService.Pdf;
using DocumentService.Pdf.Models;
using DocumentService.Word;
using DocumentService.Word.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using DocumentServiceWebAPI.Helpers;

namespace DocumentServiceWebAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        [HttpPost]
        [Route("pdf/GeneratePdfUsingHtml")]
        //public IActionResult GeneratePdf(string templatePath, List<ContentMetaData> metaDataList, string outputFilePath)
        public IActionResult GeneratePdf(PdfGenerationRequestDTO request)
        {
            try
            {
                //decoding: passed encoded base64 string to file and saving it to the temp folder and returning that template file's link
                string templatePath = Base64Helper.ConvertFileToBase64(request.Base64); 

                // Generate the PDF using the template
                string outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "convertedPdfs");
                // Creating a directory for the converted output PDFs if it doesn't exist
                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                string outputFilePath = Path.Combine(outputFolder, "output.pdf");


                // passing in main function template to pdf.
                string toolFolderAbsolutePath = "F:\\c# codes\\DocumentServiceWebAPI\\Tools\\";
                PdfDocumentGenerator.GeneratePdfByTemplate(toolFolderAbsolutePath, templatePath, request.DocumentData.Placeholders, outputFilePath);

                return Ok(new { message = "PDF generated successfully" });
                
            }
            catch (Exception ex)
            {
                return BadRequest($"PDF generation failed: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("word/GenerateWordDocument")]
        public IActionResult GenerateWord(
            [FromForm] string templateFilePath,
            [FromForm] string outputFilePath,
            [FromForm] List<ContentData> placeholders,
            [FromForm] List<TableData> tablesData)
        {
            try
            {
                DocumentService.Word.Models.DocumentData documentData = new()
                {
                    Placeholders = placeholders,
                    TablesData = tablesData
                };

                WordDocumentGenerator.GenerateDocumentByTemplate(templateFilePath, documentData, outputFilePath);
                return Ok("Word document generated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Word document generation failed: {ex.Message}");
            }
        }
    }
}
