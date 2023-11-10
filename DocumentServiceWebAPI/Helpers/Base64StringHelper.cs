namespace DocumentServiceWebAPI.Helpers;

public static class Base64StringHelper
{

    public static void SaveBase64StringToFilePath(string base64String, string filePath)
    {
        byte[] data = Convert.FromBase64String(base64String);
        File.WriteAllBytes(filePath, data);
    }

    public static string ConvertFileToBase64String(string filePath)
    {
        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            return Convert.ToBase64String(fileData);
        }
        else
        {
            throw new FileNotFoundException("The file does not exist: " + filePath);
        }
    }
}

