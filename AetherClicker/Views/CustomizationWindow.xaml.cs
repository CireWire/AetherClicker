using System.Windows;
using AetherClicker.ViewModels;

namespace AetherClicker.Views;

public partial class CustomizationWindow : Window, ICustomizationWindow
{
    private readonly GameViewModel _gameViewModel;

    public CustomizationWindow(GameViewModel gameViewModel)
    {
        InitializeComponent();
        _gameViewModel = gameViewModel;
        DataContext = new ViewModels.CustomizationViewModel(this, gameViewModel);
        Closing += CustomizationWindow_Closing;
    }

    private void CustomizationWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        // If we're not transitioning to the game window, exit the application
        if (!((CustomizationViewModel)DataContext).IsTransitioningToGame)
        {
            Application.Current.Shutdown();
        }
    }

    public new void Close()
    {
        base.Close();
    }

    public new void Show()
    {
        base.Show();
    }
}

public interface ICustomizationWindow
{
    void Close();
    void Show();
} 