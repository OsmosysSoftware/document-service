namespace DocumentServiceWebAPI.Helpers;

public static class CommonMethodsHelper
{
    public static void CreateDirectoryIfNotExists(string filePath)
    {
        // Get directory name of the file
        // If path is a file name only, directory name will be an empty string
        string directoryName = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrWhiteSpace(directoryName))
        {
            if (!Directory.Exists(directoryName))
            {
                // Create all directories on the path that don't already exist
                Directory.CreateDirectory(directoryName);
            }
        }
    }

    public static string GenerateRandomFileName(string fileExtension)
    {
        string randomFileName = Path.GetRandomFileName().Replace(".", string.Empty);
        return $"{randomFileName}-{Guid.NewGuid()}.{fileExtension}";
    }
}
