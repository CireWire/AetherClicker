using System;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;

namespace AetherClicker.Views
{
    public partial class UpdateWindow : Window
    {
        public string Version { get; set; }
        public string ReleaseNotes { get; set; }
        public string DownloadUrl { get; set; }

        public ICommand UpdateCommand { get; }
        public ICommand CloseCommand { get; }

        public UpdateWindow(string version, string releaseNotes, string downloadUrl)
        {
            InitializeComponent();
            Version = version;
            ReleaseNotes = releaseNotes;
            DownloadUrl = downloadUrl;
            DataContext = this;

            UpdateCommand = new RelayCommand(async () => await UpdateNow());
            CloseCommand = new RelayCommand(() => Close());
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

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute();
    }
} 