using DocumentService.Word;
using DocumentServiceWebAPI.Helpers;
using DocumentServiceWebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocumentServiceWebAPI.Controllers;
[Route("api")]
[ApiController]
public class WordController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public WordController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
    {
        this._configuration = configuration;
        this._hostingEnvironment = hostingEnvironment;
    }

    [HttpPost]
    [Route("word/GenerateWordDocument")]
    public ActionResult<BaseResponse> GenerateWord(WordGenerationRequestDTO request)
    {
        BaseResponse response = new BaseResponse(ResponseStatus.Fail);

        try
        {
            // Generate filepath to save base64 docx template
            string docxTemplateFilePath = Path.Combine(
                this._hostingEnvironment.WebRootPath,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:INPUT_WORD").Value,
                CommonMethodsHelper.GenerateRandomFileName("docx")
            );

            CommonMethodsHelper.CreateDirectoryIfNotExists(docxTemplateFilePath);

            // Save docx template to inputs directory
            Base64StringHelper.SaveBase64StringToFilePath(request.Base64, docxTemplateFilePath);

            // Initialize output filepath
            string outputFilePath = Path.Combine(
                this._hostingEnvironment.WebRootPath,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:OUTPUT_WORD").Value,
                CommonMethodsHelper.GenerateRandomFileName("docx")
            );

            CommonMethodsHelper.CreateDirectoryIfNotExists(outputFilePath);

            // Generate and save output docx in output directory
            WordDocumentGenerator.GenerateDocumentByTemplate(
                docxTemplateFilePath,
                request.DocumentData,
                outputFilePath
            );

            // Convert docx file in output directory to base64 string
            string outputBase64String = Base64StringHelper.ConvertFileToBase64String(outputFilePath);

            // Return response
            response.Status = ResponseStatus.Success;
            response.Base64 = outputBase64String;
            response.Message = "Word document generated successfully";
            return this.Ok(response);
        }
        catch (Exception ex)
        {
            response.Status = ResponseStatus.Error;
            response.Message = ex.Message;
            return this.StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}
