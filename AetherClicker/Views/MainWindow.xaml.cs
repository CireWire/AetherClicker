using System.Windows;
using AetherClicker.ViewModels;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Controls;
using System.Diagnostics;

namespace AetherClicker.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new GameViewModel();
        }

        private async Task CheckForUpdates()
        {
            try
            {
                var updateInfo = await VersionInfo.CheckForUpdates();
                if (updateInfo != null)
                {
                    var updateWindow = new UpdateWindow(
                        updateInfo.Version,
                        updateInfo.ReleaseNotes,
                        updateInfo.DownloadUrl
                    );
                    updateWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't show to user - update check failures shouldn't impact gameplay
                Debug.WriteLine($"Error checking for updates: {ex.Message}");
            }
        }

        protected override async void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            await CheckForUpdates();
        }
    }
} 