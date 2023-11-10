﻿using DocumentServiceWebAPI.Models;
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
            //Common Directories-----------------------------------------------------------------------------
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
            //Word specific folder structure---------------------------------------------------------------------------

                //Creating Word folder inside Input folder---------------------------------------------------------------
                string wordInputFolder = Path.Combine(inputFolder, "Word");

                //Creating Word folder if not exists inside Input folder
                if (!Directory.Exists(wordInputFolder))
                {
                    Directory.CreateDirectory(wordInputFolder);
                }
                //---------------------------------------------------------------------------------------------------------
            //Creating Word folder inside Output directory--------------------------------------------------------------
            string wordOutputFolder = Path.Combine(outputFolder, "Word");
            //Creating Word folder if not exists inside Input folder
            if (!Directory.Exists(wordOutputFolder))
            {
                Directory.CreateDirectory(wordOutputFolder);
            }
            //--------------------------------------------------------------------------------------------------------
            string ext, randomFileName, randomFileNameWithExtension;
            ext = "docx";
            randomFileName = Path.GetRandomFileName().Replace(".", string.Empty);
            randomFileNameWithExtension = $"{randomFileName}.{ext}";

            string base64FilePath = Path.Combine(wordInputFolder, randomFileNameWithExtension);

            Base64StringHelper.SaveBase64StringToFilePath(request.Base64,base64FilePath );
            //-------------------------------------------------------------------------------------
           

            //creating random name for output doc
            
            
            randomFileName = Path.GetRandomFileName().Replace(".", string.Empty);
            randomFileNameWithExtension = $"{randomFileName}.{ext}";
            string outputFilePath = Path.Combine(wordOutputFolder, randomFileNameWithExtension);


            DocumentService.Word.Models.DocumentData documentData = new()
            {
                Placeholders = request.DocumentData.Placeholders,
                TablesData = request.DocumentData.TablesData
            };

            WordDocumentGenerator.GenerateDocumentByTemplate(base64FilePath, documentData, outputFilePath);
            string base64 = Base64StringHelper.ConvertFileToBase64String(outputFilePath);

            return Ok(new { message = "Word document generated successfully.", Base64 = base64 });
        }
        catch (Exception ex)
        {
            return BadRequest($"Word document generation failed: {ex.Message}");
        }
    }
}
