using AetherClicker.Models;
using Xunit;

namespace AetherClicker.Tests;

public class PrestigeTests
{
    private GameState CreateTestGameState()
    {
        var gameState = new GameState();
        gameState.PlayerName = "TestPlayer";
        gameState.CompanyName = "TestCompany";
        gameState.SelectedBackground = "Mystic";
        gameState.SelectedSpecialization = "Alchemy";
        gameState.AddCoins(10000);
        gameState.AddMagicEssence(5000);

        // Add some game elements
        var producer = new Producer("Test Producer", "Test Description", 100, 10);
        producer.Purchase();
        producer.Purchase();
        producer.Purchase();
        producer.Purchase();
        producer.Purchase();
        gameState.AddProducer(producer);

        var upgrade = new Upgrade("Test Upgrade", "Test Description", 200, 2.0, UpgradeType.GlobalEfficiency, "producer");
        upgrade.Purchase();
        gameState.AddUpgrade(upgrade);

        var achievement = new Achievement("Test Achievement", "Test Description", 100, AchievementType.CoinsEarned, 1.1, "producer");
        achievement.IsUnlocked = true;
        gameState.AddAchievement(achievement);

        var enhancement = new Enhancement("Test Enhancement", "Test Description", 300, 2.0, EnhancementType.Efficiency);
        enhancement.Purchase();
        gameState.AddEnhancement(enhancement);

        return gameState;
    }

    [Fact]
    public void Prestige_ResetsGameStateCorrectly()
    {
        // Arrange
        var gameState = CreateTestGameState();
        gameState.AddCoins(1_000_000);
        var initialPrestigeLevel = gameState.PrestigeLevel;

        // Act
        gameState.PerformPrestige();

        // Assert
        Assert.Equal(initialPrestigeLevel + 1, gameState.PrestigeLevel);
        Assert.Equal(0, gameState.Coins);
        Assert.Equal(0, gameState.MagicEssence);
        Assert.Empty(gameState.Producers);
        Assert.Empty(gameState.Upgrades);
        Assert.Empty(gameState.Enhancements);
        // Achievements should persist
        Assert.NotEmpty(gameState.Achievements);
    }

    [Fact]
    public void Prestige_CalculatesBonusCorrectly()
    {
        // Arrange
        var gameState = CreateTestGameState();
        gameState.AddCoins(1_000_000);
        var initialPrestigeLevel = gameState.PrestigeLevel;
        var expectedBonus = 1.0 + (initialPrestigeLevel + 1) * 0.1; // 10% increase per level

        // Act
        gameState.PerformPrestige();

        // Assert
        Assert.Equal(expectedBonus, gameState.PrestigeMultiplier);
    }

    [Fact]
    public void Prestige_AppliesBonusToNewGame()
    {
        // Arrange
        var gameState = CreateTestGameState();
        gameState.AddCoins(1_000_000);
        gameState.PerformPrestige();

        // Act
        var producer = new Producer("Test Producer", "Test Description", 100, 10);
        gameState.AddProducer(producer);
        producer.Purchase();

        // Assert
        var expectedProduction = 10 * gameState.PrestigeMultiplier;
        Assert.Equal(expectedProduction, producer.CalculateProduction());
    }

    [Fact]
    public void Prestige_RequiresMinimumCoins()
    {
        // Arrange
        var gameState = CreateTestGameState();
        gameState.Coins = 100; // Below minimum

        // Act & Assert
        Assert.False(gameState.CanPrestige);
    }

    [Fact]
    public void Prestige_RequiresMinimumMagicEssence()
    {
        // Arrange
        var gameState = CreateTestGameState();
        gameState.MagicEssence = 100; // Below minimum

        // Act & Assert
        Assert.False(gameState.CanPrestige);
    }

    [Fact]
    public void Prestige_WithSufficientResources_IsAllowed()
    {
        // Arrange
        var gameState = CreateTestGameState();
        gameState.AddCoins(1_000_000);

        // Act & Assert
        Assert.True(gameState.CanPrestige);
    }

    [Fact]
    public void Prestige_CalculatesPrestigePointsCorrectly()
    {
        // Arrange
        var gameState = CreateTestGameState();
        gameState.AddCoins(1_000_000);

        // Act
        var points = gameState.CalculatePrestigePoints();

        // Assert
        Assert.True(points > 0);
        // Points should be based on total coins earned
        Assert.Equal(Math.Floor(Math.Log10(gameState.TotalCoinsEarned / 1_000_000)), points);
    }

    [Fact]
    public void Prestige_MaintainsPlayerCustomization()
    {
        // Arrange
        var gameState = CreateTestGameState();
        gameState.AddCoins(1_000_000);
        var originalName = gameState.PlayerName;
        var originalCompany = gameState.CompanyName;
        var originalBackground = gameState.SelectedBackground;
        var originalSpecialization = gameState.SelectedSpecialization;

        // Act
        gameState.PerformPrestige();

        // Assert
        Assert.Equal(originalName, gameState.PlayerName);
        Assert.Equal(originalCompany, gameState.CompanyName);
        Assert.Equal(originalBackground, gameState.SelectedBackground);
        Assert.Equal(originalSpecialization, gameState.SelectedSpecialization);
    }
} 