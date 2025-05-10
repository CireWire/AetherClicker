using AetherClicker.Models;
using Xunit;

namespace AetherClicker.Tests;

public class GameStateTests
{
    private GameState CreateTestGameState()
    {
        var gameState = new GameState();
        gameState.PlayerName = "TestPlayer";
        gameState.CompanyName = "TestCompany";
        return gameState;
    }

    [Fact]
    public void NewGame_InitializesWithCorrectValues()
    {
        // Arrange
        var gameState = CreateTestGameState();

        // Assert
        Assert.Equal("TestPlayer", gameState.PlayerName);
        Assert.Equal("TestCompany", gameState.CompanyName);
        Assert.Equal(0, gameState.Coins);
        Assert.Equal(0, gameState.MagicEssence);
        Assert.NotEmpty(gameState.Producers); // GameState initializes with default producers
        Assert.NotEmpty(gameState.Upgrades); // GameState initializes with default upgrades
        Assert.NotEmpty(gameState.Achievements); // GameState initializes with default achievements
    }

    [Fact]
    public void MakeTrade_IncreasesCoins()
    {
        // Arrange
        var gameState = CreateTestGameState();
        var initialCoins = gameState.Coins;
        var expectedIncrease = gameState.ClickValue * gameState.Prestige.ClickValueMultiplier;

        // Act
        gameState.MakeTrade();

        // Assert
        Assert.Equal(initialCoins + expectedIncrease, gameState.Coins);
    }

    [Fact]
    public void TryPurchaseProducer_WithSufficientCoins_ReturnsTrue()
    {
        // Arrange
        var gameState = CreateTestGameState();
        var producer = gameState.Producers[0]; // Get first producer
        gameState.Coins = producer.CurrentCost * 2; // Ensure enough coins
        var initialQuantity = producer.Quantity;

        // Act
        var result = gameState.TryPurchaseProducer(producer);

        // Assert
        Assert.True(result);
        Assert.Equal(initialQuantity + 1, producer.Quantity);
    }

    [Fact]
    public void TryPurchaseProducer_WithInsufficientCoins_ReturnsFalse()
    {
        // Arrange
        var gameState = CreateTestGameState();
        var producer = gameState.Producers[0];
        gameState.Coins = producer.CurrentCost / 2; // Not enough coins
        var initialQuantity = producer.Quantity;

        // Act
        var result = gameState.TryPurchaseProducer(producer);

        // Assert
        Assert.False(result);
        Assert.Equal(initialQuantity, producer.Quantity);
    }

    [Fact]
    public void TryPurchaseUpgrade_WithSufficientCoins_ReturnsTrue()
    {
        // Arrange
        var gameState = CreateTestGameState();
        var upgrade = gameState.Upgrades[0];
        gameState.Coins = upgrade.CurrentCost * 2;
        var initialLevel = upgrade.Level;

        // Act
        var result = gameState.TryPurchaseUpgrade(upgrade);

        // Assert
        Assert.True(result);
        Assert.Equal(initialLevel + 1, upgrade.Level);
    }

    [Fact]
    public void TryPurchaseUpgrade_WithInsufficientCoins_ReturnsFalse()
    {
        // Arrange
        var gameState = CreateTestGameState();
        var upgrade = gameState.Upgrades[0];
        gameState.Coins = upgrade.CurrentCost / 2;
        var initialLevel = upgrade.Level;

        // Act
        var result = gameState.TryPurchaseUpgrade(upgrade);

        // Assert
        Assert.False(result);
        Assert.Equal(initialLevel, upgrade.Level);
    }

    [Fact]
    public void TryPurchaseEnhancement_WithSufficientMagicEssence_ReturnsTrue()
    {
        // Arrange
        var gameState = CreateTestGameState();
        var producer = gameState.Producers[0];
        var enhancement = new Enhancement("Test Enhancement", "Test Description", 100, 1.5, EnhancementType.Efficiency);
        gameState.MagicEssence = enhancement.BaseCost * 2;

        // Act
        var result = gameState.TryPurchaseEnhancement(producer, enhancement);

        // Assert
        Assert.True(result);
        Assert.Contains(enhancement, producer.Enhancements);
    }

    [Fact]
    public void TryPurchaseEnhancement_WithInsufficientMagicEssence_ReturnsFalse()
    {
        // Arrange
        var gameState = CreateTestGameState();
        var producer = gameState.Producers[0];
        var enhancement = new Enhancement("Test Enhancement", "Test Description", 100, 1.5, EnhancementType.Efficiency);
        gameState.MagicEssence = enhancement.BaseCost / 2;

        // Act
        var result = gameState.TryPurchaseEnhancement(producer, enhancement);

        // Assert
        Assert.False(result);
        Assert.DoesNotContain(enhancement, producer.Enhancements);
    }
} 