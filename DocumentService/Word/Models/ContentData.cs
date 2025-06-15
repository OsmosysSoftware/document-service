namespace DocumentService.Word.Models;


/// <summary>
/// Represents the data for a content placeholder in a Word document.
/// </summary>
public class ContentData
{
    /// <summary>
    /// Gets or sets the placeholder name.
    /// </summary>
    public string Placeholder { get; set; }

    /// <summary>
    /// Gets or sets the content to replace the placeholder with.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Gets or sets the content type of the placeholder (text or image).
    /// </summary>
    public ContentType ContentType { get; set; }

    /// <summary>
    /// Gets or sets the parent body of the placeholder (none or table).
    /// </summary>

    public ParentBody ParentBody { get; set; }
}
