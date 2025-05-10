using AetherClicker.Models;
using AetherClicker.ViewModels;
using AetherClicker.Views;
using System.Windows.Input;
using Xunit;
using System.Windows.Threading;

namespace AetherClicker.Tests;

[Collection("UI")]
public class CustomizationViewModelTests
{
    private CustomizationViewModel CreateTestCustomizationViewModel()
    {
        var gameViewModel = new GameViewModel();
        // Instead of creating a real window, we'll use a mock or test double
        var window = new TestCustomizationWindow(gameViewModel);
        return new CustomizationViewModel(window, gameViewModel);
    }

    // Test window implementation that doesn't require STA thread
    private class TestCustomizationWindow : ICustomizationWindow
    {
        private readonly GameViewModel _gameViewModel;

        public TestCustomizationWindow(GameViewModel gameViewModel)
        {
            _gameViewModel = gameViewModel;
        }

        public void Close()
        {
            // No-op for tests
        }

        public void Show()
        {
            // No-op for tests
        }
    }

    [Fact]
    public void NewCustomizationViewModel_InitializesWithCorrectValues()
    {
        // Arrange & Act
        var viewModel = CreateTestCustomizationViewModel();

        // Assert
        Assert.NotNull(viewModel.PlayerName);
        Assert.NotNull(viewModel.CompanyName);
        Assert.NotNull(viewModel.SelectedBackground);
        Assert.NotNull(viewModel.SelectedSpecialization);
        Assert.NotNull(viewModel.AvailableBackgrounds);
        Assert.NotNull(viewModel.AvailableSpecializations);
    }

    [Fact]
    public void SetPlayerName_UpdatesGameState()
    {
        // Arrange
        var viewModel = CreateTestCustomizationViewModel();
        var newName = "New Player Name";

        // Act
        viewModel.PlayerName = newName;

        // Assert
        Assert.Equal(newName, viewModel.PlayerName);
    }

    [Fact]
    public void SetCompanyName_UpdatesGameState()
    {
        // Arrange
        var viewModel = CreateTestCustomizationViewModel();
        var newName = "New Company Name";

        // Act
        viewModel.CompanyName = newName;

        // Assert
        Assert.Equal(newName, viewModel.CompanyName);
    }

    [Fact]
    public void SetBackground_UpdatesGameState()
    {
        // Arrange
        var viewModel = CreateTestCustomizationViewModel();
        var newBackground = new CustomizationOption("Mystic", "A mystical background");

        // Act
        viewModel.SelectedBackground = newBackground;

        // Assert
        Assert.Equal(newBackground, viewModel.SelectedBackground);
    }

    [Fact]
    public void SetSpecialization_UpdatesGameState()
    {
        // Arrange
        var viewModel = CreateTestCustomizationViewModel();
        var newSpecialization = new CustomizationOption("Alchemy", "Expert in potion brewing");

        // Act
        viewModel.SelectedSpecialization = newSpecialization;

        // Assert
        Assert.Equal(newSpecialization, viewModel.SelectedSpecialization);
    }

    [Fact]
    public void StartGameCommand_IsInitialized()
    {
        // Arrange & Act
        var viewModel = CreateTestCustomizationViewModel();

        // Assert
        Assert.NotNull(viewModel.StartGameCommand);
    }

    [Fact]
    public void StartGameCommand_CanExecute_WithValidInput()
    {
        // Arrange
        var viewModel = CreateTestCustomizationViewModel();
        viewModel.PlayerName = "Test Player";
        viewModel.CompanyName = "Test Company";
        viewModel.SelectedBackground = new CustomizationOption("Mystic", "A mystical background");
        viewModel.SelectedSpecialization = new CustomizationOption("Alchemy", "Expert in potion brewing");

        // Act & Assert
        Assert.True(((ICommand)viewModel.StartGameCommand).CanExecute(null));
    }

    [Fact]
    public void StartGameCommand_CannotExecute_WithInvalidInput()
    {
        // Arrange
        var viewModel = CreateTestCustomizationViewModel();
        viewModel.PlayerName = "";
        viewModel.CompanyName = "";
        viewModel.SelectedBackground = null;
        viewModel.SelectedSpecialization = null;

        // Act & Assert
        Assert.False(((ICommand)viewModel.StartGameCommand).CanExecute(null));
    }

    [Fact]
    public void AvailableBackgrounds_ContainsExpectedOptions()
    {
        // Arrange & Act
        var viewModel = CreateTestCustomizationViewModel();

        // Assert
        Assert.Contains("Default", viewModel.AvailableBackgrounds);
        Assert.Contains("Dark", viewModel.AvailableBackgrounds);
        Assert.Contains("Light", viewModel.AvailableBackgrounds);
    }

    [Fact]
    public void AvailableSpecializations_ContainsExpectedOptions()
    {
        // Arrange & Act
        var viewModel = CreateTestCustomizationViewModel();

        // Assert
        Assert.Contains("None", viewModel.AvailableSpecializations);
        Assert.Contains("Producer", viewModel.AvailableSpecializations);
        Assert.Contains("Upgrade", viewModel.AvailableSpecializations);
        Assert.Contains("Enhancement", viewModel.AvailableSpecializations);
    }
} 