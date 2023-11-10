using DocumentService.Pdf;
using DocumentServiceWebAPI.Helpers;
using DocumentServiceWebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocumentServiceWebAPI.Controllers;


[Route("api")]
[ApiController]
public class PdfController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public PdfController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
    {
        this._configuration = configuration;
        this._hostingEnvironment = hostingEnvironment;
    }

    [HttpPost]
    [Route("pdf/GeneratePdfUsingHtml")]
    public IActionResult GeneratePdf(PdfGenerationRequestDTO request)
    {
        try
        {
            // Generate filepath to save base64 html template
            string htmlTemplateFilePath = Path.Combine(
                this._hostingEnvironment.WebRootPath,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:INPUT_HTML").Value,
                CommonMethodsHelper.GenerateRandomFileName("html")
            );

            CommonMethodsHelper.CreateDirectoryIfNotExists(htmlTemplateFilePath);

            // Save base64 html template to inputs directory
            Base64StringHelper.SaveBase64StringToFilePath(request.Base64, htmlTemplateFilePath);

            // Initialize tools and output filepaths
            string htmlToPDfToolsFilePath = Path.Combine(
                this._hostingEnvironment.WebRootPath,
                this._configuration.GetSection("STATIC_FILE_PATHS:HTML_TO_PDF_TOOL").Value
            );

            string outputFilePath = Path.Combine(
                this._hostingEnvironment.WebRootPath,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:OUTPUT_HTML").Value,
                CommonMethodsHelper.GenerateRandomFileName("pdf")
            );

            CommonMethodsHelper.CreateDirectoryIfNotExists(outputFilePath);

            // Generate and save pdf in output directory
            PdfDocumentGenerator.GeneratePdfByTemplate(
                htmlToPDfToolsFilePath,
                htmlTemplateFilePath,
                request.DocumentData.Placeholders,
                outputFilePath
            );

            // Convert pdf file in output directory to base64 string
            string outputBase64String = Base64StringHelper.ConvertFileToBase64String(outputFilePath);

            // Return response
            return this.Ok(new
            {
                message = "PDF generated successfully",
                Base64 = outputBase64String
            });
        }
        catch (Exception ex)
        {
            return this.BadRequest($"PDF generation failed: {ex.Message}");
        }
    }
}
