namespace DocumentServiceWebAPI.Helpers;

public static class Base64StringHelper
{
    public static async Task SaveBase64StringToFilePath(string base64String, string filePath)
    {
        byte[] data = Convert.FromBase64String(base64String);
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
