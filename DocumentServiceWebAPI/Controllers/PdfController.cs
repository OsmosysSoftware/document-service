using DocumentServiceWebAPI.Models;
using DocumentService.Pdf;
using Microsoft.AspNetCore.Mvc;
using DocumentServiceWebAPI.Helpers;

namespace DocumentServiceWebAPI.Controllers;
[Route("api")]
[ApiController]
public class PdfController : ControllerBase
{
    [HttpPost]
    [Route("pdf/GeneratePdfUsingHtml")]
    public IActionResult GeneratePdf(PdfGenerationRequestDTO request)
    {
        try
        {
            // 1. Build the file path where the temporary HTML file will be stored.
            // 2. Convert the base64 to file. The file should be stored in the temp directory decided in step #2.
            // 3. Pass the file to the Document Service for conversion. The file should be stored in the temp location.
            // 4. Convert the output file to base64 and send it as a response.

            //decoding: passed encoded base64 string to file and saving it to the temp folder and returning that template file's link
            string templatePath = Base64Helper.ConvertBase64ToFile(request.Base64, "pdf");

            // Generate the PDF using the template
            string outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "convertedPdfs");
            // Creating a directory for the converted output PDFs if it doesn't exist
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            string outputFilePath = Path.Combine(outputFolder, "pdfOutput.pdf");


            // passing in main function template to pdf.
            string toolFolderAbsolutePath = "F:\\Desktop\\document-service\\DocumentServiceWebAPI\\Tools\\wkhtmltopdf.exe";
            PdfDocumentGenerator.GeneratePdfByTemplate(toolFolderAbsolutePath, templatePath, request.DocumentData.Placeholders, outputFilePath);
            string base64 = Base64Helper.ConvertFileToBase64(outputFilePath);
            return Ok(new { message = "PDF generated successfully", Base64 = base64 });

        }
        catch (Exception ex)
        {
            return BadRequest($"PDF generation failed: {ex.Message}");
        }
    }
}
