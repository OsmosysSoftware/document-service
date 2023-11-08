namespace DocumentServiceWebAPI.Helpers
{
    public static class Base64Helper
    {

        public static string ConvertBase64ToFile(string encodedFile, string flag) 
        {
            byte[] data = Convert.FromBase64String(encodedFile);

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Remove the "bin\Debug\net7.0" part from the base directory path
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
            string tempFilePath;
            if (flag =="pdf") { 
                tempFilePath = Path.Combine(decodedDirectory, "decodedFile.html"); 
            }
            else
            {
                tempFilePath = Path.Combine(decodedDirectory, "decodedFile.docx");
            }
            
            File.WriteAllBytes(tempFilePath, data);
            Console.WriteLine("File has been saved to the 'decodedFile' directory at location " + tempFilePath);

            return tempFilePath;
        } 

        public static string ConvertFileToBase64 (string outputFilePath)
        {
            if (File.Exists(outputFilePath))
            {
                byte[] fileData = File.ReadAllBytes(outputFilePath);
                string base64String = Convert.ToBase64String(fileData);
                return base64String;
            }
            else
            {
                throw new FileNotFoundException("The file does not exist: " + outputFilePath);
            }
        }
    }
}
