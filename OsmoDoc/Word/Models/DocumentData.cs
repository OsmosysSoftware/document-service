using System;
using System.Collections.Generic;
using System.Text;

namespace OsmoDoc.Word.Models;


/// <summary>
/// Represents the data for a Word document, including content placeholders and table data.
/// </summary>
public class DocumentData
{
    /// <summary>
    /// Gets or sets the list of content placeholders in the document.
    /// </summary>
    public List<ContentData> Placeholders { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of table data in the document.
    /// </summary>

    public List<TableData> TablesData { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of images in the document.
    /// </summary>
    public List<ImageData> Images { get; set; } = new();
}
