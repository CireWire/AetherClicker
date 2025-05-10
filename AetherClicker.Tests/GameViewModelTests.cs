using AetherClicker.Models;
using AetherClicker.ViewModels;
using System.Windows.Input;
using Xunit;

namespace AetherClicker.Tests;

public class GameViewModelTests
{
    private GameViewModel CreateTestGameViewModel()
    {
        var gameState = new GameState
        {
            PlayerName = "TestPlayer",
            CompanyName = "TestCompany",
            SelectedBackground = "Mystic",
            SelectedSpecialization = "Alchemy"
        };
        return new GameViewModel(gameState);
    }

    [Fact]
    public void NewGameViewModel_InitializesWithCorrectValues()
    {
        // Arrange & Act
        var viewModel = CreateTestGameViewModel();

        // Assert
        Assert.Equal("TestPlayer", viewModel.PlayerName);
        Assert.Equal("TestCompany", viewModel.CompanyName);
        Assert.Equal("Mystic", viewModel.SelectedBackground);
        Assert.Equal("Alchemy", viewModel.SelectedSpecialization);
        Assert.Equal(0, viewModel.Coins);
        Assert.Equal(0, viewModel.MagicEssence);
        Assert.Empty(viewModel.Producers);
        Assert.Empty(viewModel.Upgrades);
        Assert.Empty(viewModel.Achievements);
        Assert.Empty(viewModel.Enhancements);
    }

    [Fact]
    public void AddProducer_WithValidProducer_AddsToCollection()
    {
        // Arrange
        var viewModel = CreateTestGameViewModel();
        var producer = new Producer("Test Producer", "Test Description", 100.0, 10.0);

        // Act
        viewModel.AddProducer(producer);

        // Assert
        Assert.Contains(producer, viewModel.Producers);
    }

    [Fact]
    public void AddProducer_WithNullProducer_ThrowsArgumentNullException()
    {
        // Arrange
        var viewModel = CreateTestGameViewModel();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.AddProducer(null!));
    }

    [Fact]
    public void AddUpgrade_WithValidUpgrade_AddsToCollection()
    {
        // Arrange
        var viewModel = CreateTestGameViewModel();
        var upgrade = new Upgrade("Test Upgrade", "Test Description", 100.0, 1.5, UpgradeType.GlobalEfficiency, "producer");

        // Act
        viewModel.AddUpgrade(upgrade);

        // Assert
        Assert.Contains(upgrade, viewModel.Upgrades);
    }

    [Fact]
    public void AddUpgrade_WithNullUpgrade_ThrowsArgumentNullException()
    {
        // Arrange
        var viewModel = CreateTestGameViewModel();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.AddUpgrade(null!));
    }

    [Fact]
    public void AddEnhancement_WithValidEnhancement_AddsToCollection()
    {
        // Arrange
        var viewModel = CreateTestGameViewModel();
        var enhancement = new Enhancement("Test Enhancement", "Test Description", 100.0, 1.5, EnhancementType.Efficiency);

        // Act
        viewModel.AddEnhancement(enhancement);

        // Assert
        Assert.Contains(enhancement, viewModel.AvailableEnhancements);
    }

    [Fact]
    public void AddEnhancement_WithNullEnhancement_ThrowsArgumentNullException()
    {
        // Arrange
        var viewModel = CreateTestGameViewModel();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.AddEnhancement(null!));
    }

    [Fact]
    public void PurchaseProducer_WithSufficientCoins_PurchasesProducer()
    {
        // Arrange
        var viewModel = CreateTestGameViewModel();
        var producer = new Producer("Test Producer", "Test Description", 100.0, 10.0);
        viewModel.AddProducer(producer);
        viewModel.AddCoins(200);

        // Act
        viewModel.PurchaseProducer(producer);

        // Assert
        Assert.Equal(1, producer.Quantity);
        Assert.Equal(100, viewModel.Coins);
    }

    [Fact]
    public void PurchaseProducer_WithInsufficientCoins_DoesNotPurchaseProducer()
    {
        // Arrange
        var viewModel = CreateTestGameViewModel();
        var producer = new Producer("Test Producer", "Test Description", 100.0, 10.0);
        viewModel.AddProducer(producer);
        viewModel.AddCoins(50);

        // Act
        viewModel.PurchaseProducer(producer);

        // Assert
        Assert.Equal(0, producer.Quantity);
        Assert.Equal(50, viewModel.Coins);
    }

    [Fact]
    public void PurchaseUpgrade_WithSufficientCoins_AddsUpgrade()
    {
        // Arrange
        var viewModel = CreateTestGameViewModel();
        var upgrade = new Upgrade("Test Upgrade", "Test Description", 100.0, 1.5, UpgradeType.GlobalEfficiency, "producer");
        viewModel.AddUpgrade(upgrade);
        viewModel.AddCoins(200);

        // Act
        viewModel.PurchaseUpgrade(upgrade);

        // Assert
        Assert.True(upgrade.IsPurchased);
        Assert.Equal(100, viewModel.Coins); // 200 - 100 cost
    }

    [Fact]
    public void PurchaseEnhancement_WithSufficientMagicEssence_AddsEnhancement()
    {
        // Arrange
        var viewModel = CreateTestGameViewModel();
        var enhancement = new Enhancement("Test Enhancement", "Test Description", 100.0, 1.5, EnhancementType.Efficiency);
        viewModel.AddEnhancement(enhancement);
        viewModel.AddMagicEssence(200);

        // Act
        viewModel.PurchaseEnhancement(enhancement);

        // Assert
        Assert.True(enhancement.IsPurchased);
        Assert.Equal(100, viewModel.MagicEssence); // 200 - 100 cost
    }

    [Fact]
    public void Commands_AreInitialized()
    {
        // Arrange & Act
        var viewModel = CreateTestGameViewModel();

        // Assert
        Assert.NotNull(viewModel.PurchaseProducerCommand);
        Assert.NotNull(viewModel.PurchaseUpgradeCommand);
        Assert.NotNull(viewModel.PurchaseEnhancementCommand);
        Assert.NotNull(viewModel.PrestigeCommand);
        Assert.NotNull(viewModel.SaveGameCommand);
        Assert.NotNull(viewModel.LoadGameCommand);
    }

    [Fact]
    public void Commands_CanExecute_WithValidConditions()
    {
        // Arrange
        var viewModel = CreateTestGameViewModel();
        var producer = new Producer("Test Producer", "Test Description", 100.0, 10.0);
        viewModel.AddProducer(producer);
        viewModel.AddCoins(200);

        // Act & Assert
        Assert.True(((ICommand)viewModel.PurchaseProducerCommand).CanExecute(producer));
    }

    [Fact]
    public void Commands_CannotExecute_WithInvalidConditions()
    {
        // Arrange
        var viewModel = CreateTestGameViewModel();
        var producer = new Producer("Test Producer", "Test Description", 100.0, 10.0);
        viewModel.AddProducer(producer);
        viewModel.AddCoins(50);

        // Act & Assert
        Assert.False(((ICommand)viewModel.PurchaseProducerCommand).CanExecute(producer));
    }
} 