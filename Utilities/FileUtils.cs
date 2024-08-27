using Newtonsoft.Json;
using Serilog;

namespace HAI_Selenium.Utilities
{
    internal static class FileUtils
    {
        internal static T LoadJsonFile<T>(string filePath)
        {
            Log.Information("[ACTION] Loading JSON file from {FilePath}...", filePath);

            try
            {
                var json = File.ReadAllText(filePath);
                var data = JsonConvert.DeserializeObject<T>(json);
                Log.Information("[SUCCESS] JSON file loaded and parsed successfully.");
                return data;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load JSON file: {Message}", ex.Message);
                throw;
            }
        }

        internal static void VerifyDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Log.Error("Directory not found: {Path}", path);
                throw new DirectoryNotFoundException($"Directory not found: {path}");
            }
        }
    }
}
