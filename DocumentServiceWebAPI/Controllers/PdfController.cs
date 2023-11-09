using DocumentServiceWebAPI.Models;
using DocumentService.Pdf;
using Microsoft.AspNetCore.Mvc;
using DocumentServiceWebAPI.Helpers;
using static ICSharpCode.SharpZipLib.Zip.ExtendedUnixData;

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
            //Common Directories-----------------------------------------------------------------------------
            //Building the basic paths and directory required
            // 1. Build the file path where the temporary HTML file will be stored.

            string rootFolder = Path.Combine(Directory.GetCurrentDirectory(), "Temp");
            string inputFolder = Path.Combine(rootFolder, "Input");
            string outputFolder = Path.Combine(rootFolder, "Output");

            //Creating Temp folder if not exists
            if (!Directory.Exists(rootFolder))
            {
                Directory.CreateDirectory(rootFolder);
            }
            //Creating Input folder if not exists
            if (!Directory.Exists(inputFolder))
            {
                Directory.CreateDirectory(inputFolder);
            }
            
                //Creating Output folder if not exists
                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }
            //---------------------------------------------------------------------------------------------------------
            //HTML&Pdf specific folder structure---------------------------------------------------------------------------

            //Creating html folder inside Input folder---------------------------------------------
            string htmlFolder = Path.Combine(inputFolder, "Html");

                //Creating Html folder if not exists inside Input folder
                if (!Directory.Exists(htmlFolder))
                {
                    Directory.CreateDirectory(htmlFolder);
                }
            //------------------------------------------------------------------------------

            //Creating Pdf folder inside Output folder--------------------------
            string pdfFolder = Path.Combine(outputFolder, "Pdf");
                

                if (!Directory.Exists(pdfFolder))
                {
                    Directory.CreateDirectory(pdfFolder);
                }
            //----------------------------------------------------------------


            //input base64 decoded file path
            string ext, randomFileName, randomFileNameWithExtension;

            ext = "html";
            randomFileName = Path.GetRandomFileName().Replace(".", string.Empty);
            randomFileNameWithExtension = $"{randomFileName}.{ext}";

            string base64FilePath = Path.Combine(htmlFolder, randomFileNameWithExtension);
            //intermediate modified file path
            //string modifiedFilePath = Path.Combine(htmlFolder, "intermediateFile.html");

            //output pdf File path
            ext = "pdf";
            randomFileName = Path.GetRandomFileName().Replace(".", string.Empty);
            randomFileNameWithExtension = $"{randomFileName}.{ext}";

            string outputFilePath = Path.Combine(pdfFolder, randomFileNameWithExtension);




                // 2. Convert the base64 to file. The file should be stored in the temp directory decided in step #2.
                string templatePath = Base64Helper.ConvertBase64ToFile(request.Base64, base64FilePath);

                // 3. Pass the file to the Document Service for conversion. The file should be stored in the temp location.
                // passing in main function template to pdf.
                string toolFolderAbsolutePath = "F:\\Desktop\\DS-Final2\\document-service\\DocumentServiceWebAPI\\Tools\\wkhtmltopdf.exe";
                PdfDocumentGenerator.GeneratePdfByTemplate(toolFolderAbsolutePath, templatePath, request.DocumentData.Placeholders, outputFilePath);

                // 4. Convert the output file to base64 and send it as a response.
                string base64 = Base64Helper.ConvertFileToBase64(outputFilePath);
                return Ok(new { message = "PDF generated successfully", Base64 = base64 });


            
        }
        catch (Exception ex)
        {
            return BadRequest($"PDF generation failed: {ex.Message}");
        }
    }
}
