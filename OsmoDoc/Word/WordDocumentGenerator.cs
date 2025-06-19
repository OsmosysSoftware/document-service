using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OsmoDoc.Word.Models;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using IOPath = System.IO.Path;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;

namespace OsmoDoc.Word;

/// <summary>
/// Provides functionality to generate Word documents based on templates and data.
/// </summary>
public static class WordDocumentGenerator
{    
    /// <summary>
    /// Generates a Word document based on a template, replaces placeholders with data, and saves it to the specified output file path.
    /// </summary>
    /// <param name="templateFilePath">The file path of the template document.</param>
    /// <param name="documentData">The data to replace the placeholders in the template.</param>
    /// <param name="outputFilePath">The file path to save the generated document.</param>
    public async static Task GenerateDocumentByTemplate(string templateFilePath, DocumentData documentData, string outputFilePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(templateFilePath))
            {
                throw new ArgumentNullException(nameof(templateFilePath));
            }

            if (string.IsNullOrWhiteSpace(outputFilePath))
            {
                throw new ArgumentNullException(nameof(outputFilePath));
            }

            List<ContentData> contentData = documentData.Placeholders;
            List<TableData> tablesData = documentData.TablesData;

            // Creating dictionaries for each type of placeholders
            Dictionary<string, string> textPlaceholders = new Dictionary<string, string>();
            Dictionary<string, string> tableContentPlaceholders = new Dictionary<string, string>();
            Dictionary<string, string> imagePlaceholders = new Dictionary<string, string>();

            foreach (ContentData content in contentData)
            {
                if (content.ParentBody == ParentBody.None && content.ContentType == ContentType.Text)
                {
                    string placeholder = "{" + content.Placeholder + "}";
                    textPlaceholders.TryAdd(placeholder, content.Content);
                }
                else if (content.ParentBody == ParentBody.None && content.ContentType == ContentType.Image)
                {
                    string placeholder = content.Placeholder;
                    imagePlaceholders.TryAdd(placeholder, content.Content);
                }
                else if (content.ParentBody == ParentBody.Table && content.ContentType == ContentType.Text)
                {
                    string placeholder = "{" + content.Placeholder + "}";
                    tableContentPlaceholders.TryAdd(placeholder, content.Content);
                }
            }

            // Create document of the template
            XWPFDocument document = await GetXWPFDocument(templateFilePath);

            // For each element in the document
            foreach (IBodyElement element in document.BodyElements)
            {
                if (element.ElementType == BodyElementType.PARAGRAPH)
                {
                    // If element is a paragraph
                    XWPFParagraph paragraph = (XWPFParagraph)element;

                    // If the paragraph is empty string or the placeholder regex does not match then continue
                    if (paragraph.ParagraphText == string.Empty || !new Regex(@"{[a-zA-Z]+}").IsMatch(paragraph.ParagraphText))
                    {
                        continue;
                    }

                    // Replace placeholders in paragraph with values
                    paragraph = ReplacePlaceholdersOnBody(paragraph, textPlaceholders);
                }
                else if (element.ElementType == BodyElementType.TABLE)
                {
                    // If element is a table
                    XWPFTable table = (XWPFTable)element;

                    // Replace placeholders in a table
                    table = ReplacePlaceholderOnTables(table, tableContentPlaceholders);

                    // Populate the table with data if it is passed in tablesData list
                    foreach (TableData insertData in tablesData)
                    {
                        if (insertData.TablePos >= 1 && insertData.TablePos <= document.Tables.Count && table == document.Tables[insertData.TablePos - 1])
                        {
                            table = PopulateTable(table, insertData);
                        }
                    }
                }
            }

            // Write the document to output file path and close the document
            WriteDocument(document, outputFilePath);
            document.Close();

            /*
             * Image Replacement is done after writing the document here,
             * because for Text Replacement, NPOI package is being used
             * and for Image Replacement, OpeXML package is used.
             * Since both the packages have different execution method, so they are handled separately
             */
            // Replace all the image placeholders in the output file
            await ReplaceImagePlaceholders(outputFilePath, outputFilePath, imagePlaceholders);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Retrieves an instance of XWPFDocument from the specified document file path.
    /// </summary>
    /// <param name="docFilePath">The file path of the Word document.</param>
    /// <returns>An instance of XWPFDocument representing the Word document.</returns>
    private async static Task<XWPFDocument> GetXWPFDocument(string docFilePath)
    {
        byte[] fileBytes = await File.ReadAllBytesAsync(docFilePath);
        using MemoryStream memoryStream = new MemoryStream(fileBytes);
        return new XWPFDocument(memoryStream);
    }

    /// <summary>
    /// Writes the XWPFDocument to the specified file path.
    /// </summary>
    /// <param name="document">The XWPFDocument to write.</param>
    /// <param name="filePath">The file path to save the document.</param>
    private static void WriteDocument(XWPFDocument document, string filePath)
    {
        string? directory = IOPath.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using (FileStream writeStream = File.Create(filePath))
        {
            document.Write(writeStream);
        }
    }

    /// <summary>
    /// Replaces the text placeholders in a paragraph with the specified values.
    /// </summary>
    /// <param name="paragraph">The XWPFParagraph containing the placeholders.</param>
    /// <param name="textPlaceholders">The dictionary of text placeholders and their corresponding values.</param>
    /// <returns>The updated XWPFParagraph.</returns>
    private static XWPFParagraph ReplacePlaceholdersOnBody(XWPFParagraph paragraph, Dictionary<string, string> textPlaceholders)
    {
        // Get a list of all placeholders in the current paragraph
        List<string> placeholdersTobeReplaced = Regex.Matches(paragraph.ParagraphText, @"{[a-zA-Z]+}")
                                                                 .Cast<Match>()
                                                                 .Select(s => s.Groups[0].Value).ToList();

        // For each placeholder in paragraph
        foreach (string placeholder in placeholdersTobeReplaced)
        {
            // Replace text placeholders in paragraph with values
            if (textPlaceholders.ContainsKey(placeholder))
            {
                paragraph.ReplaceText(placeholder, textPlaceholders[placeholder]);
            }

            paragraph.SpacingAfter = 0;
        }

        return paragraph;
    }

    /// <summary>
    /// Replaces the text placeholders in a table with the specified values.
    /// </summary>
    /// <param name="table">The XWPFTable containing the placeholders.</param>
    /// <param name="tableContentPlaceholders">The dictionary of table content placeholders and their corresponding values.</param>
    /// <returns>The updated XWPFTable.</returns>
    private static XWPFTable ReplacePlaceholderOnTables(XWPFTable table, Dictionary<string, string> tableContentPlaceholders)
    {
        // Loop through each cell of the table
        foreach (XWPFTableRow row in table.Rows)
        {
            foreach (XWPFTableCell cell in row.GetTableCells())
            {
                foreach (XWPFParagraph paragraph in cell.Paragraphs)
                {
                    // Get a list of all placeholders in the current cell
                    List<string> placeholdersTobeReplaced = Regex.Matches(paragraph.ParagraphText, @"{[a-zA-Z]+}")
                                                             .Cast<Match>()
                                                             .Select(s => s.Groups[0].Value).ToList();

                    // For each placeholder in the cell
                    foreach (string placeholder in placeholdersTobeReplaced)
                    {
                        // replace the placeholder with its value
                        if (tableContentPlaceholders.ContainsKey(placeholder))
                        {
                            paragraph.ReplaceText(placeholder, tableContentPlaceholders[placeholder]);
                        }
                    }
                }
            }
        }

        return table;
    }

    /// <summary>
    /// Populates a table with the specified data.
    /// </summary>
    /// <param name="table">The XWPFTable to populate.</param>
    /// <param name="tableData">The data to populate the table.</param>
    /// <returns>The updated XWPFTable.</returns>
    private static XWPFTable PopulateTable(XWPFTable table, TableData tableData)
    {
        // Get the header row
        XWPFTableRow headerRow = table.GetRow(0);

        // Return if no header row found or if it does not have any column
        if (headerRow == null || headerRow.GetTableCells() == null || headerRow.GetTableCells().Count <= 0)
        {
            return table;
        }

        // For each row's data stored in table data
        foreach (Dictionary<string, string> rowData in tableData.Data)
        {
            XWPFTableRow row = table.CreateRow(); // This is a DATA row, not header
            
            int columnCount = headerRow.GetTableCells().Count; // Read from header
            for (int cellNumber = 0; cellNumber < columnCount; cellNumber++)
            {
                // Ensure THIS data row has enough cells
                while (row.GetTableCells().Count <= cellNumber)
                {
                    row.AddNewTableCell();
                }

                // Now populate the cell in this data row
                XWPFTableCell cell = row.GetCell(cellNumber);
                string columnHeader = headerRow.GetCell(cellNumber).GetText();
                if (rowData.ContainsKey(columnHeader))
                {
                    cell.SetText(rowData[columnHeader]);
                }
            }
        }

        return table;
    }

    /// <summary>
    /// Replaces the image placeholders in the output file with the specified images.
    /// </summary>
    /// <param name="inputFilePath">The input file path containing the image placeholders.</param>
    /// <param name="outputFilePath">The output file path where the updated document will be saved.</param>
    /// <param name="imagePlaceholders">The dictionary of image placeholders and their corresponding image paths.</param>
    private async static Task ReplaceImagePlaceholders(string inputFilePath, string outputFilePath, Dictionary<string, string> imagePlaceholders)
    {
        byte[] docBytes = await File.ReadAllBytesAsync(inputFilePath);

        // Write document bytes to memory asynchronously
        using (MemoryStream memoryStream = new MemoryStream())
        {
            await memoryStream.WriteAsync(docBytes, 0, docBytes.Length);
            memoryStream.Position = 0;

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(memoryStream, true))
            {
                MainDocumentPart? mainDocumentPart = wordDocument.MainDocumentPart;

                // Get a list of drawings (images)
                IEnumerable<Drawing> drawings = new List<Drawing>();
                if (mainDocumentPart != null)
                {
                    drawings = mainDocumentPart.Document.Descendants<Drawing>().ToList();
                }

                /* 
                 * FIXME: Look on how we can improve this loop operation.
                 */
                foreach (Drawing drawing in drawings)
                {
                    DocProperties? docProperty = drawing.Descendants<DocProperties>().FirstOrDefault();

                    // If drawing / image name is present in imagePlaceholders dictionary, then replace image
                    if (docProperty != null && imagePlaceholders.ContainsKey(docProperty.Name))
                    {
                        List<Blip> drawingBlips = drawing.Descendants<Blip>().ToList();

                        foreach (Blip blip in drawingBlips)
                        {
                            OpenXmlPart imagePart = wordDocument.MainDocumentPart.GetPartById(blip.Embed);

                            string imagePath = imagePlaceholders[docProperty.Name];

                            // Asynchronously download image data using HttpClient
                            using HttpClient httpClient = new HttpClient();
                            byte[] imageData = await httpClient.GetByteArrayAsync(imagePath);

                            using (Stream partStream = imagePart.GetStream(FileMode.OpenOrCreate, FileAccess.Write))
                            {
                                // Asynchronously write image data to the part stream
                                await partStream.WriteAsync(imageData, 0, imageData.Length);
                                partStream.SetLength(imageData.Length); // Ensure the stream is truncated if new data is smaller
                            }
                        }
                    }
                }
            }
            // Overwrite the output file asynchronously
            using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            {
                // Reset MemoryStream position before writing to fileStream
                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(fileStream);
            }
        }
    }
}