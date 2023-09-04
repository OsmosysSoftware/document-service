using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using DocumentService.Pdf.Models;
using System.Diagnostics;

namespace DocumentService.Pdf
{
    public class FilePathDoesNotExist : Exception
    {
        public FilePathDoesNotExist(string msg) : base(msg)
        {

        }
    }
     
    public class PdfDocumentGenerator
    {
        public static void generatePdfByTemplate(string templatePath, List<ContentMetaData> metaDataList, string outputFilePath)
        {
            try 
            { 
                if(!File.Exists(templatePath))
                {
                    throw new FilePathDoesNotExist("The file path you provided is not Valid"); 
                }
                
                string modifiedHtmlFilePath =  ReplaceFileElementsWithMetaData(templatePath, metaDataList);
                
                ConversionCmd(modifiedHtmlFilePath, outputFilePath);

            } 
            catch(Exception e)
            {
                throw;
            }
        }

        private static string ReplaceFileElementsWithMetaData(string templatePath, List<ContentMetaData> metaDataList)
        {
            string htmlContent = File.ReadAllText(templatePath);
            foreach (ContentMetaData metaData in metaDataList)
            {
                htmlContent = htmlContent.Replace("{" + metaData.Placeholder + "}", metaData.Content);
            

            }
            
            Console.WriteLine("Current: "+ Directory.GetCurrentDirectory());
            string tempHtmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Temp");
            string tempHtmlFile = Path.Combine(tempHtmlFilePath, "modifiedHtml.html");

            if (!Directory.Exists(tempHtmlFilePath))
            {
                Directory.CreateDirectory(tempHtmlFilePath);
            }

            File.WriteAllText(tempHtmlFile, htmlContent);
            
            return tempHtmlFile;
        }

        private static void ConversionCmd(string modifiedHtmlFilePath, string outputFilePath)
        {
            string wkHtmlToPdfPath = "cmd.exe";

            string arguments = $"/C Tools\\wkhtmltopdf.exe \"{modifiedHtmlFilePath}\" \"{outputFilePath}\"";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = wkHtmlToPdfPath, // Path to the executable file of wkhtmlpdfpath.
                Arguments = arguments, // Path to modified template and output location.
                RedirectStandardOutput = true, // To capture output.
                RedirectStandardError = true, // To capture error messages.
                UseShellExecute = false, 
                CreateNoWindow = true
            };

            

            using (Process process = new Process())
            {
                process.StartInfo = psi; // Provide StartInfo with info regarding starting process.
                process.Start();
                
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();
                Console.WriteLine("Output: " + output);
                Console.WriteLine("Errors: " + errors);
            }

            File.Delete(modifiedHtmlFilePath);

        }
    }
}
