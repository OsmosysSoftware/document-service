using System.Collections.Generic;

namespace OsmoDoc.Word.Models;


/// <summary>
/// Represents the data for a table in a Word document.
/// </summary>
public class TableData
{
    /// <summary>
    /// Gets or sets the position of the table in the document.
    /// </summary>
    public int TablePos { get; set; }

    /// <summary>
    /// Gets or sets the list of dictionaries representing the data for each row in the table.
    /// Each dictionary contains column header-value pairs.
    /// </summary>

    public List<Dictionary<string, string>> Data { get; set; }
}
