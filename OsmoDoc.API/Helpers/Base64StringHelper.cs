namespace OsmoDoc.API.Helpers;

public static class Base64StringHelper
{
    public static async Task SaveBase64StringToFilePath(string base64String, string filePath, IConfiguration configuration)
    {
        byte[] data = Convert.FromBase64String(base64String);

        long uploadFileSizeLimitBytes = Convert.ToInt64(configuration.GetSection("CONFIG:UPLOAD_FILE_SIZE_LIMIT_BYTES").Value);

        if (data.LongLength > uploadFileSizeLimitBytes)
        {
            throw new BadHttpRequestException("Uploaded file is too large");
        }

        await File.WriteAllBytesAsync(filePath, data);
    }

    public static async Task<string> ConvertFileToBase64String(string filePath)
    {
        if (File.Exists(filePath))
        {
            byte[] fileData = await File.ReadAllBytesAsync(filePath);
            return Convert.ToBase64String(fileData);
        }
        else
        {
            throw new FileNotFoundException("The file does not exist: " + filePath);
        }
    }
}
