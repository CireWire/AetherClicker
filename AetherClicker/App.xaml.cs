using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;
using AetherClicker.ViewModels;
using AetherClicker.Views;

namespace AetherClicker;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Handle any unhandled exceptions
        Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        // Create and show the customization window
        var gameViewModel = new AetherClicker.ViewModels.GameViewModel();
        var customizationWindow = new AetherClicker.Views.CustomizationWindow(gameViewModel);
        customizationWindow.Show();
    }

    private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show($"An error occurred: {e.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            MessageBox.Show($"A critical error occurred: {ex.Message}", "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Clean up any resources here
        base.OnExit(e);
    }
}

