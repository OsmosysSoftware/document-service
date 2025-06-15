namespace DocumentService.Word.Models;


/// <summary>
/// Represents the content type of a placeholder in a Word document.
/// </summary>
public enum ContentType
{
    /// <summary>
    /// The placeholder represents text content.
    /// </summary>
    Text = 0,

    /// <summary>
    /// The placeholder represents an image.
    /// </summary>
    Image = 1
}

/// <summary>
/// Represents the parent body of a placeholder in a Word document.
/// </summary>
public enum ParentBody
{
    /// <summary>
    /// The placeholder does not have a parent body.
    /// </summary>
    None = 0,

    /// <summary>
    /// The placeholder belongs to a table.
    /// </summary>

    Table = 1
}
