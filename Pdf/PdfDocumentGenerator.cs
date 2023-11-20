using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using DocumentService.Pdf.Models;
using System.Diagnostics;

namespace DocumentService.Pdf
{
    public class PdfDocumentGenerator
    {
        public static void GeneratePdf(string toolFolderAbsolutePath, string templatePath, List<ContentMetaData> metaDataList, string outputFilePath, bool isEjsTemplate)
        {
            try
            {
                if (!File.Exists(templatePath))
                {
                    throw new Exception("The file path you provided is not valid.");
                }

                // If input template is an ejs file then convert ejs file to an equivalent html
                if (isEjsTemplate)
                {
                    templatePath = ConvertEjsToHTML(templatePath, outputFilePath);
                }

                // Modify html template with content data and generate pdf
                string modifiedHtmlFilePath = ReplaceFileElementsWithMetaData(templatePath, metaDataList, outputFilePath);
                ConvertHtmlToPdf(toolFolderAbsolutePath, modifiedHtmlFilePath, outputFilePath);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static string ReplaceFileElementsWithMetaData(string templatePath, List<ContentMetaData> metaDataList, string outputFilePath)
        {
            string htmlContent = File.ReadAllText(templatePath);

            foreach (ContentMetaData metaData in metaDataList)
            {
                htmlContent = htmlContent.Replace($"{{{metaData.Placeholder}}}", metaData.Content);
            }

            string directoryPath = Path.GetDirectoryName(outputFilePath);
            string tempHtmlFilePath = Path.Combine(directoryPath, "Temp");
            string tempHtmlFile = Path.Combine(tempHtmlFilePath, "modifiedHtml.html");

            if (!Directory.Exists(tempHtmlFilePath))
            {
                Directory.CreateDirectory(tempHtmlFilePath);
            }

            File.WriteAllText(tempHtmlFile, htmlContent);
            return tempHtmlFile;
        }

        private static void ConvertHtmlToPdf(string toolFolderAbsolutePath, string modifiedHtmlFilePath, string outputFilePath)
        {
            string wkHtmlToPdfPath = "cmd.exe";
            string arguments = $"/C {toolFolderAbsolutePath} \"{modifiedHtmlFilePath}\" \"{outputFilePath}\"";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = wkHtmlToPdfPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = psi;
                process.Start();
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();
            }

            File.Delete(modifiedHtmlFilePath);
        }

        private static string ConvertEjsToHTML(string ejsFilePath, string outputFilePath)
        {
            string directoryPath = Path.GetDirectoryName(outputFilePath);
            string tempHtmlFilePath = Path.Combine(directoryPath, "Temp");

            if (!Directory.Exists(tempHtmlFilePath))
            {
                Directory.CreateDirectory(tempHtmlFilePath);
            }

            tempHtmlFilePath = Path.Combine(tempHtmlFilePath, "htmlTemplate.html");

            string commandLine = "cmd.exe";
            string arguments = $"/C ejs \"{ejsFilePath}\" -o \"{tempHtmlFilePath}\"";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = commandLine,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = psi;
                process.Start();
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();
            }


            return tempHtmlFilePath;
        }
    }
}
