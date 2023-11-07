namespace DocumentServiceWebAPI.Helpers
{
    public static class Base64Helper
    {

        public static string ConvertFileToBase64(string encodedFile) 
        {
            byte[] data = Convert.FromBase64String(encodedFile);

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Remove the "bin\Debug\net6.0" part from the base directory path
            int binIndex = baseDirectory.IndexOf("bin");
            if (binIndex >= 0)
            {
                baseDirectory = baseDirectory.Substring(0, binIndex);
            }

            // Create a "decodedFolder" directory within the base directory if it doesn't exist
            string decodedDirectory = Path.Combine(baseDirectory, "decodedFolder");
            if (!Directory.Exists(decodedDirectory))
            {
                Directory.CreateDirectory(decodedDirectory);
            }
            string tempFilePath = Path.Combine(decodedDirectory, "decodedFile.html");
            File.WriteAllBytes(tempFilePath, data);
            Console.WriteLine("File has been saved to the 'decodedFile' directory at location " + tempFilePath);

            return tempFilePath;
        } 

        public static string ConvertBase64ToFile(string base64, string outputFilePath)
        {
            return String.Empty;
        }
    }
}
