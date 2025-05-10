using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Diagnostics;
using System.Reflection;

namespace AetherClicker.Utils
{
    public class VersionInfo
    {
        private const string VERSION_CHECK_URL = "https://api.github.com/repos/CireWire/AetherClicker/releases/latest";
        private static readonly HttpClient _httpClient = new HttpClient();

        public string Version { get; set; } = string.Empty;
        public string ReleaseNotes { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }

        public static string CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0.0";

        public static async Task<VersionInfo?> CheckForUpdates()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "AetherClicker");
                var response = await _httpClient.GetStringAsync(VERSION_CHECK_URL);
                var releaseInfo = JsonSerializer.Deserialize<VersionInfo>(response);

                if (releaseInfo != null && IsNewVersionAvailable(CurrentVersion, releaseInfo.Version))
                {
                    return releaseInfo;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking for updates: {ex.Message}");
            }
            return null;
        }

        private static bool IsNewVersionAvailable(string currentVersion, string latestVersion)
        {
            try
            {
                var current = System.Version.Parse(currentVersion);
                var latest = System.Version.Parse(latestVersion);
                return latest > current;
            }
            catch
            {
                return false;
            }
        }
    }
} 