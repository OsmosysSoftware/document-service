using DocumentService.Pdf.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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

                if (isEjsTemplate)
                {
                    // Validate if template in file path is an ejs file
                    if (Path.GetExtension(templatePath).ToLower() != ".ejs")
                    {
                        throw new Exception("Input template should be a valid EJS file");
                    }

                    // Convert ejs file to an equivalent html
                    templatePath = ConvertEjsToHTML(templatePath, outputFilePath);
                }

                // Modify html template with content data and generate pdf
                string modifiedHtmlFilePath = ReplaceFileElementsWithMetaData(templatePath, metaDataList, outputFilePath);
                ConvertHtmlToPdf(toolFolderAbsolutePath, modifiedHtmlFilePath, outputFilePath);

                if (isEjsTemplate)
                {
                    // If input template was an ejs file, then the template path contains path to html converted from ejs
                    if (Path.GetExtension(templatePath).ToLower() == ".html")
                    {
                        // If template path contains path to converted html template then delete it
                        File.Delete(templatePath);
                    }
                }
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

            /*
             * FIXME: Issue if tools file path has spaces in between
             */
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
