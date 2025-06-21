using System.ComponentModel.DataAnnotations;

public enum ImageSourceType
{
    Base64,
    LocalFile,
    Url
}

public class ImageData
{
    [Required(ErrorMessage = "Placeholder name is required")]
    public string PlaceholderName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Image source type is required")]
    public ImageSourceType SourceType { get; set; }

    [Required(ErrorMessage = "Image data is required")]
    public string Data { get; set; } = string.Empty; // Can be base64, file path, or URL

    [Required(ErrorMessage = "Image extension is required")]
    public string? ImageExtension { get; set; } // Required for Base64
}