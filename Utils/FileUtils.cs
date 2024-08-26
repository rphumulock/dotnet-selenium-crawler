using Newtonsoft.Json;

namespace HAI_Selenium.Utils
{
    internal static class FileUtils
    {
        internal static T LoadJsonFile<T>(string filePath)
        {
            Console.WriteLine("[ACTION] Loading JSON file...");

            try
            {
                var json = File.ReadAllText(filePath);
                var data = JsonConvert.DeserializeObject<T>(json);
                Console.WriteLine("[SUCCESS] JSON file loaded and parsed.");
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load JSON file: {ex.Message}");
                throw;
            }
        }

        internal static void VerifyDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"[ERROR] Directory not found: {path}");
            }
        }
    }
}
