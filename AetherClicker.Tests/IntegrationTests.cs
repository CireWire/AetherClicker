using AetherClicker.Models;
using Xunit;

namespace AetherClicker.Tests;

public class IntegrationTests
{
    private GameState CreateTestGameState()
    {
        return new GameState
        {
            PlayerName = "TestPlayer",
            CompanyName = "TestCompany",
            SelectedBackground = "Mystic",
            SelectedSpecialization = "Alchemy"
        };
    }

    [Fact]
    public void ProducerAndUpgrade_Interaction_WorksCorrectly()
    {
        // Arrange
        var gameState = CreateTestGameState();
        var producer = new Producer("Test Producer", "A test producer", 100, 10);
        var upgrade = new Upgrade("Production Boost", "Doubles production", 50, 2.0, UpgradeType.GlobalEfficiency, "producer");

        // Act
        gameState.AddProducer(producer);
        gameState.AddUpgrade(upgrade);
        gameState.AddCoins(200); // Enough for both producer and upgrade
        producer.Purchase();
        upgrade.Purchase();
        upgrade.ApplyEffect(gameState);

        // Assert
        Assert.Equal(1, producer.Quantity);
        Assert.True(upgrade.IsPurchased);
        Assert.Equal(20, producer.CalculateProduction()); // Should be doubled due to upgrade
    }

    [Fact]
    public void ProducerAndAchievement_Interaction_WorksCorrectly()
    {
        // Arrange
        var gameState = CreateTestGameState();
        var producer = new Producer("Test Producer", "A test producer", 100, 10);
        var achievement = new Achievement("First Producer", "Purchase your first producer", 1.0, AchievementType.CoinsEarned, 1.0, "producer");

        // Act
        gameState.AddProducer(producer);
        gameState.AddAchievement(achievement);
        gameState.AddCoins(100);
        producer.Purchase();
        achievement.UpdateProgress(gameState);

        // Assert
        Assert.Equal(1, producer.Quantity);
        Assert.True(achievement.IsUnlocked);
    }

    [Fact]
    public void EnhancementAndProducer_Interaction_WorksCorrectly()
    {
        // Arrange
        var gameState = CreateTestGameState();
        var producer = new Producer("Test Producer", "A test producer", 100, 10);
        var enhancement = new Enhancement("Efficiency Boost", "Reduces producer cost by 20%", 50, 0.2, EnhancementType.Efficiency);

        // Act
        gameState.AddProducer(producer);
        gameState.AddEnhancement(enhancement);
        gameState.AddMagicEssence(100);
        enhancement.Purchase();
        enhancement.ApplyEffect(producer);

        // Assert
        Assert.True(enhancement.IsPurchased);
        Assert.True(producer.CalculateCost() < 100); // Cost should be reduced
    }

    [Fact]
    public void MultipleComponents_Interaction_WorksCorrectly()
    {
        // Arrange
        var gameState = CreateTestGameState();
        var producer = new Producer("Test Producer", "A test producer", 100, 10);
        var upgrade = new Upgrade("Production Boost", "Doubles production", 50, 2.0, UpgradeType.GlobalEfficiency, "producer");
        var enhancement = new Enhancement("Efficiency Boost", "Reduces producer cost by 20%", 50, 0.2, EnhancementType.Efficiency);

        // Act
        gameState.AddProducer(producer);
        gameState.AddUpgrade(upgrade);
        gameState.AddEnhancement(enhancement);
        gameState.AddCoins(200);
        gameState.AddMagicEssence(100);

        producer.Purchase();
        upgrade.Purchase();
        enhancement.Purchase();

        upgrade.ApplyEffect(gameState);
        enhancement.ApplyEffect(producer);

        // Assert
        Assert.Equal(1, producer.Quantity);
        Assert.True(upgrade.IsPurchased);
        Assert.True(enhancement.IsPurchased);
        Assert.Equal(20, producer.CalculateProduction()); // Doubled by upgrade
        Assert.True(producer.CalculateCost() < 100); // Reduced by enhancement
    }
} 