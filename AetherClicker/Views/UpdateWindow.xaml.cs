using System;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;
using System.ComponentModel;
using AetherClicker.Commands;

namespace AetherClicker.Views
{
    public partial class UpdateWindow : Window
    {
        public string Version { get; set; } = string.Empty;
        public string ReleaseNotes { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;

        public ICommand UpdateCommand { get; }
        public ICommand CloseCommand { get; }

        public UpdateWindow(string version, string releaseNotes, string downloadUrl)
        {
            InitializeComponent();
            Version = version;
            ReleaseNotes = releaseNotes;
            DownloadUrl = downloadUrl;
            DataContext = this;

            UpdateCommand = new RelayCommand(async _ => await UpdateNow());
            CloseCommand = new RelayCommand(_ => Close());
        }

        private async Task UpdateNow()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = DownloadUrl,
                    UseShellExecute = true
                });
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting update: {ex.Message}", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 