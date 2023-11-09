namespace DocumentServiceWebAPI.Helpers;

public static class Base64Helper
{

    public static string ConvertBase64ToFile(string base64String, string base64FilePath) 
    {
        byte[] data = Convert.FromBase64String(base64String);            
        File.WriteAllBytes(base64FilePath, data);
        Console.WriteLine("File has been saved at location " + base64FilePath);
        return base64FilePath;
    } 

    public static string ConvertFileToBase64 (string filePath)
    {
        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            string base64String = Convert.ToBase64String(fileData);
            return base64String;
        }
        else
        {
            throw new FileNotFoundException("The file does not exist: " + filePath);
        }
    }
}

