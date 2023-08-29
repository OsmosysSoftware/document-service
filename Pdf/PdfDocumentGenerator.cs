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
                    throw new FilePathDoesNotExist("The file path you provided is not Valid"); // Throw error message in case file does not exist.
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
            string htmlContent = File.ReadAllText(htmlFilePath);

            foreach(ContentMetaData metaData in metaDataList)
            {
                htmlContent = htmlContent.Replace("{" + metaData.Placeholder + "}", metaData.Content);
            }

            string tempHtmlFilePath = Path.GetTempFileName();
            File.WriteAllText(tempHtmlFilePath, htmlContent);

            return tempHtmlFilePath;
        }

        private static void ConversionCmd(string modifiedHtmlFilePath, string outputFilePath)
        {
            string wkhtmltopdfPath = @"D:\wkhtmltopdf\bin\wkhtmltopdf.exe";

            string arguments = $"\"{modifiedHtmlFilePath}\" \"{outputFilePath}\"";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = wkhtmltopdfPath, // Path to the executable file of wkhtmlpdfpath.
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
            }

        }
    }
}
