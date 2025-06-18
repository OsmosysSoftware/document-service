using OsmoDoc.Pdf;
using OsmoDoc.API.Helpers;
using OsmoDoc.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace OsmoDoc.API.Controllers;

[Route("api")]
[ApiController]
public class PdfController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly ILogger<PdfController> _logger;

    public PdfController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, ILogger<PdfController> logger)
    {
        this._configuration = configuration;
        this._hostingEnvironment = hostingEnvironment;
        this._logger = logger;
    }

    [HttpPost]
    [Authorize]
    [Route("pdf/GeneratePdfUsingHtml")]
    public async Task<ActionResult<BaseResponse>> GeneratePdf(PdfGenerationRequestDTO request)
    {
        BaseResponse response = new BaseResponse(ResponseStatus.Fail);

        try
        {
            // Generate filepath to save base64 html template
            string htmlTemplateFilePath = Path.Combine(
                this._hostingEnvironment.WebRootPath,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:TEMP").Value,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:INPUT").Value,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:HTML").Value,
                CommonMethodsHelper.GenerateRandomFileName("html")
            );

            CommonMethodsHelper.CreateDirectoryIfNotExists(htmlTemplateFilePath);

            // Save base64 html template to inputs directory
            await Base64StringHelper.SaveBase64StringToFilePath(request.Base64, htmlTemplateFilePath, this._configuration);

            // Initialize tools and output filepaths
            OsmoDocPdfConfig.WkhtmltopdfPath = Path.Combine(
                this._hostingEnvironment.WebRootPath,
                this._configuration.GetSection("STATIC_FILE_PATHS:HTML_TO_PDF_TOOL").Value
            );

            string outputFilePath = Path.Combine(
                this._hostingEnvironment.WebRootPath,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:TEMP").Value,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:OUTPUT").Value,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:PDF").Value,
                CommonMethodsHelper.GenerateRandomFileName("pdf")
            );

            CommonMethodsHelper.CreateDirectoryIfNotExists(outputFilePath);

            // Generate and save pdf in output directory
            await PdfDocumentGenerator.GeneratePdf(
                htmlTemplateFilePath,
                request.DocumentData.Placeholders,
                outputFilePath,
                isEjsTemplate: false,
                serializedEjsDataJson: null
            );

            // Convert pdf file in output directory to base64 string
            string outputBase64String = await Base64StringHelper.ConvertFileToBase64String(outputFilePath);

            // Return response
            response.Status = ResponseStatus.Success;
            response.Base64 = outputBase64String;
            response.Message = "PDF generated successfully";
            return this.Ok(response);
        }
        catch (BadHttpRequestException ex)
        {
            response.Status = ResponseStatus.Error;
            response.Message = ex.Message;
            this._logger.LogError(ex.Message);
            this._logger.LogError(ex.StackTrace);
            return this.BadRequest(response);
        }
        catch (FormatException ex)
        {
            response.Status = ResponseStatus.Error;
            response.Message = "Error converting base64 string to file";
            this._logger.LogError(ex.Message);
            this._logger.LogError(ex.StackTrace);
            return this.BadRequest(response);
        }
        catch (FileNotFoundException ex)
        {
            response.Status = ResponseStatus.Error;
            response.Message = "Unable to load file saved from base64 string";
            this._logger.LogError(ex.Message);
            this._logger.LogError(ex.StackTrace);
            return this.StatusCode(StatusCodes.Status500InternalServerError, response);
        }
        catch (Exception ex)
        {
            response.Status = ResponseStatus.Error;
            response.Message = ex.Message;
            this._logger.LogError(ex.Message);
            this._logger.LogError(ex.StackTrace);
            return this.StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    [HttpPost]
    [Authorize]
    [Route("pdf/GeneratePdfUsingEjs")]
    public async Task<ActionResult<BaseResponse>> GeneratePdfUsingEjs(PdfGenerationRequestDTO request)
    {
        BaseResponse response = new BaseResponse(ResponseStatus.Fail);

        try
        {
            // Generate filepath to save base64 html template
            string ejsTemplateFilePath = Path.Combine(
                this._hostingEnvironment.WebRootPath,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:TEMP").Value,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:INPUT").Value,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:EJS").Value,
                CommonMethodsHelper.GenerateRandomFileName("ejs")
            );

            CommonMethodsHelper.CreateDirectoryIfNotExists(ejsTemplateFilePath);

            // Save base64 html template to inputs directory
            await Base64StringHelper.SaveBase64StringToFilePath(request.Base64, ejsTemplateFilePath, this._configuration);

            // Initialize tools and output filepaths
            OsmoDocPdfConfig.WkhtmltopdfPath = Path.Combine(
                this._hostingEnvironment.WebRootPath,
                this._configuration.GetSection("STATIC_FILE_PATHS:HTML_TO_PDF_TOOL").Value
            );

            string outputFilePath = Path.Combine(
                this._hostingEnvironment.WebRootPath,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:TEMP").Value,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:OUTPUT").Value,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:PDF").Value,
                CommonMethodsHelper.GenerateRandomFileName("pdf")
            );

            CommonMethodsHelper.CreateDirectoryIfNotExists(outputFilePath);

            // Generate and save pdf in output directory
            await PdfDocumentGenerator.GeneratePdf(
                ejsTemplateFilePath,
                request.DocumentData?.Placeholders,
                outputFilePath,
                isEjsTemplate: true,
                serializedEjsDataJson: request.SerializedEjsDataJson
            );

            // Convert pdf file in output directory to base64 string
            string outputBase64String = await Base64StringHelper.ConvertFileToBase64String(outputFilePath);

            // Return response
            response.Status = ResponseStatus.Success;
            response.Base64 = outputBase64String;
            response.Message = "PDF generated successfully";
            return this.Ok(response);
        }
        catch (BadHttpRequestException ex)
        {
            response.Status = ResponseStatus.Error;
            response.Message = ex.Message;
            this._logger.LogError(ex.Message);
            this._logger.LogError(ex.StackTrace);
            return this.BadRequest(response);
        }
        catch (FormatException ex)
        {
            response.Status = ResponseStatus.Error;
            response.Message = "Error converting base64 string to file";
            this._logger.LogError(ex.Message);
            this._logger.LogError(ex.StackTrace);
            return this.BadRequest(response);
        }
        catch (FileNotFoundException ex)
        {
            response.Status = ResponseStatus.Error;
            response.Message = "Unable to load file saved from base64 string";
            this._logger.LogError(ex.Message);
            this._logger.LogError(ex.StackTrace);
            return this.StatusCode(StatusCodes.Status500InternalServerError, response);
        }
        catch (Exception ex)
        {
            response.Status = ResponseStatus.Error;
            response.Message = ex.Message;
            this._logger.LogError(ex.Message);
            this._logger.LogError(ex.StackTrace);
            return this.StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}
