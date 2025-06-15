namespace OsmoDoc.API;

public static class DotEnv
{
    public static void Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        foreach (string line in File.ReadAllLines(filePath))
        {
            // Check if the line contains '='
            int equalsIndex = line.IndexOf('=');
            if (equalsIndex == -1)
            {
                continue; // Skip lines without '='
            }

            string key = line.Substring(0, equalsIndex).Trim();
            string value = line.Substring(equalsIndex + 1).Trim();

            // Check if the value starts and ends with double quotation marks
            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                // Remove the double quotation marks
                value = value[1..^1];
            }

            Environment.SetEnvironmentVariable(key, value);
        }
    }

}
