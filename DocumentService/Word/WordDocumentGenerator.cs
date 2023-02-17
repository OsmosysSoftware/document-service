using NPOI.XWPF.UserModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using DocumentService.Word.Models;

namespace DocumentService.Word
{
    public static class WordDocumentGenerator
    {
        public static void GenerateDocumentByTemplate(string templateFilePath, DocumentData documentData, string outputFilePath)
        {
            try
            {
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
                        textPlaceholders.Add(placeholder, content.Content);
                    }
                    else if (content.ParentBody == ParentBody.None && content.ContentType == ContentType.Image)
                    {
                        string placeholder = "{" + content.Placeholder + "}";
                        imagePlaceholders.Add(placeholder, content.Content);
                    }
                    else if (content.ParentBody == ParentBody.Table && content.ContentType == ContentType.Text)
                    {
                        string placeholder = "{" + content.Placeholder + "}";
                        tableContentPlaceholders.Add(placeholder, content.Content);
                    }
                }

                // Create document of the template
                XWPFDocument document = GetXWPFDocument(templateFilePath);

                // For each element in the document
                foreach (var element in document.BodyElements)
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
                        paragraph = ReplacePlaceholdersOnBody(paragraph, textPlaceholders, imagePlaceholders);
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
                            if (insertData.TablePos <= document.Tables.Count && table == document.Tables[insertData.TablePos - 1])
                            {
                                table = PopulateTable(table, insertData);
                            }
                        }
                    }
                }

                // Write the document to output file path and close the document
                WriteDocument(document, outputFilePath);
                document.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private static XWPFDocument GetXWPFDocument(string docFilePath)
        {
            FileStream readStream = File.OpenRead(docFilePath);
            XWPFDocument document = new XWPFDocument(readStream);
            return document;
        }

        private static void WriteDocument(XWPFDocument document, string filePath)
        {
            using (FileStream writeStream = File.Create(filePath))
            {
                document.Write(writeStream);
            }
        }

        private static XWPFParagraph ReplacePlaceholdersOnBody(XWPFParagraph paragraph, Dictionary<string, string> textPlaceholders, Dictionary<string, string> imagePlaceholders)
        {
            // Get a list of all placeholders in the current paragraph
            List<string> placeholdersTobeReplaced = Regex.Matches(paragraph.ParagraphText, @"{[a-zA-Z]+}")
                                                                     .Cast<Match>()
                                                                     .Select(s => s.Groups[0].Value).ToList();

            // For each placeholder in paragraph
            foreach (string placeholder in placeholdersTobeReplaced)
            {
                if (new Regex(@"{[a-zA-Z]+Image}").IsMatch(paragraph.ParagraphText))
                {
                    // Replace image placeholders in paragraph with Images
                    paragraph = ReplacePlaceholderWithImage(paragraph, placeholder, imagePlaceholders);
                }
                else
                {
                    // Replace text placeholders in paragraph with values
                    if (textPlaceholders.ContainsKey(placeholder))
                    {
                        paragraph.ReplaceText(placeholder, textPlaceholders[placeholder]);
                    }
                    paragraph.SpacingAfter = 0;
                }
            }

            return paragraph;
        }

        private static XWPFParagraph ReplacePlaceholderWithImage(XWPFParagraph paragraph, string placeholder, Dictionary<string, string> imagePlaceholders)
        {
            // Remove the image's placeholder
            paragraph.ReplaceText(placeholder, "");

            // Create a run in the paragraph for the image
            XWPFRun imageRun = paragraph.CreateRun();

            // Read the image and Add it in placeholder's place
            using (FileStream imageData = new FileStream(imagePlaceholders[placeholder], FileMode.Open, FileAccess.Read))
            {
                var widthEmus = (int)(85.0 * 9525);
                var heightEmus = (int)(85.0 * 9525);
                imageRun.AddPicture(imageData, (int)PictureType.PNG, "", widthEmus, heightEmus);
            }

            return paragraph;
        }

        private static XWPFTable ReplacePlaceholderOnTables(XWPFTable table, Dictionary<string, string> tableContentPlaceholders)
        {
            // Get a count of rows
            int rowCount = table.NumberOfRows;

            // Return if no rows found
            if (rowCount <= 0)
            {
                return table;
            }

            // Get a count of columns
            int columnCount = table.Rows[0].GetTableCells().Count;

            // Loop through each cell of the table
            foreach (XWPFTableRow row in table.Rows)
            {
                foreach (XWPFTableCell cell in row.GetTableCells())
                {
                    if (cell.Paragraphs.Count <= 0)
                    {
                        continue;
                    }

                    XWPFParagraph paragraph = cell.Paragraphs[0];

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

            return table;
        }

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
                // Create a new row and its columns
                XWPFTableRow row = table.CreateRow();

                // For each cell in row
                for (int cellNumber = 0; cellNumber < row.GetTableCells().Count; cellNumber++)
                {
                    XWPFTableCell cell = row.GetCell(cellNumber);

                    // Get the column header of this cell
                    string columnHeader = headerRow.GetCell(cellNumber).GetText();

                    // Add the cell's value
                    if (rowData.ContainsKey(columnHeader))
                    {
                        cell.SetText(rowData[columnHeader]);
                    }
                }
            }

            return table;
        }
    }
}
