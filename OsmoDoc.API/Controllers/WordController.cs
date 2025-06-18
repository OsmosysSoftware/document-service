using AutoMapper;
using OsmoDoc.Word;
using OsmoDoc.Word.Models;
using OsmoDoc.API.Helpers;
using OsmoDoc.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace OsmoDoc.API.Controllers;

[Route("api")]
[ApiController]
public class WordController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly ILogger<WordController> _logger;
    private readonly IMapper _mapper;

    public WordController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, ILogger<WordController> logger, IMapper mapper)
    {
        this._configuration = configuration;
        this._hostingEnvironment = hostingEnvironment;
        this._logger = logger;
        this._mapper = mapper;
    }

    [HttpPost]
    [Authorize]
    [Route("word/GenerateWordDocument")]
    public async Task<ActionResult<BaseResponse>> GenerateWord(WordGenerationRequestDTO request)
    {
        BaseResponse response = new BaseResponse(ResponseStatus.Fail);

        try
        {
            // Generate filepath to save base64 docx template
            string docxTemplateFilePath = Path.Combine(
                this._hostingEnvironment.WebRootPath,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:TEMP").Value,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:INPUT").Value,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:WORD").Value,
                CommonMethodsHelper.GenerateRandomFileName("docx")
            );

            CommonMethodsHelper.CreateDirectoryIfNotExists(docxTemplateFilePath);

            // Save docx template to inputs directory
            await Base64StringHelper.SaveBase64StringToFilePath(request.Base64, docxTemplateFilePath, this._configuration);

            // Initialize output filepath
            string outputFilePath = Path.Combine(
                this._hostingEnvironment.WebRootPath,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:TEMP").Value,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:OUTPUT").Value,
                this._configuration.GetSection("TEMPORARY_FILE_PATHS:WORD").Value,
                CommonMethodsHelper.GenerateRandomFileName("docx")
            );

            CommonMethodsHelper.CreateDirectoryIfNotExists(outputFilePath);

            // Handle image placeholder data in request
            foreach (WordContentDataRequestDTO placeholder in request.DocumentData.Placeholders)
            {
                if (placeholder.ContentType == ContentType.Image)
                {
                    if (string.IsNullOrWhiteSpace(placeholder.ImageExtension))
                    {
                        throw new BadHttpRequestException("Image extension is required for image content data");
                    }

                    if (string.IsNullOrWhiteSpace(placeholder.Content))
                    {
                        throw new BadHttpRequestException("Image content data is required");
                    }

                    // Remove '.' from image extension if present
                    placeholder.ImageExtension = placeholder.ImageExtension.Replace(".", string.Empty);

                    // Generate a random image file name and its path
                    string imageFilePath = Path.Combine(
                        this._hostingEnvironment.WebRootPath,
                        this._configuration.GetSection("TEMPORARY_FILE_PATHS:TEMP").Value,
                        this._configuration.GetSection("TEMPORARY_FILE_PATHS:INPUT").Value,
                        this._configuration.GetSection("TEMPORARY_FILE_PATHS:WORD").Value,
                        this._configuration.GetSection("TEMPORARY_FILE_PATHS:IMAGES").Value,
                        CommonMethodsHelper.GenerateRandomFileName(placeholder.ImageExtension)
                    );

                    CommonMethodsHelper.CreateDirectoryIfNotExists(imageFilePath);

                    // Save image content base64 string to inputs directory
                    await Base64StringHelper.SaveBase64StringToFilePath(placeholder.Content, imageFilePath, this._configuration);

                    // Replace placeholder content with image file path
                    placeholder.Content = imageFilePath;
                }
            }

            // Map document data in request to word library model class
            DocumentData documentData = new DocumentData
            {
                Placeholders = this._mapper.Map<List<ContentData>>(request.DocumentData.Placeholders),
                TablesData = request.DocumentData.TablesData
            };

            // Generate and save output docx in output directory
            await WordDocumentGenerator.GenerateDocumentByTemplate(
                docxTemplateFilePath,
                documentData,
                outputFilePath
            );

            // Convert docx file in output directory to base64 string
            string outputBase64String = await Base64StringHelper.ConvertFileToBase64String(outputFilePath);

            // Return response
            response.Status = ResponseStatus.Success;
            response.Base64 = outputBase64String;
            response.Message = "Word document generated successfully";
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
