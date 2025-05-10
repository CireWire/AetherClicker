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
        public string Version { get; set; } = "1.0.0";
        public string DownloadUrl { get; set; } = "";
        public string ReleaseNotes { get; set; } = "";
        public DateTime ReleaseDate { get; set; }

        private const string VERSION_CHECK_URL = "https://api.github.com/repos/CireWire/AetherClicker/releases/latest";

        public static string CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public static async Task<VersionInfo> CheckForUpdates()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "AetherClicker");
                    var response = await client.GetStringAsync(VERSION_CHECK_URL);
                    var releaseInfo = JsonSerializer.Deserialize<VersionInfo>(response);

                    if (IsNewVersionAvailable(CurrentVersion, releaseInfo.Version))
                    {
                        return releaseInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking for updates: {ex.Message}");
            }

            return null;
        }

        public static bool IsNewVersionAvailable(string currentVersion, string latestVersion)
        {
            try
            {
                var current = Version.Parse(currentVersion);
                var latest = Version.Parse(latestVersion);
                return latest > current;
            }
            catch
            {
                return false;
            }
        }
    }
} 