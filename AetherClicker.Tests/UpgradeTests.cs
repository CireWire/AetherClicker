using AetherClicker.Models;
using Xunit;

namespace AetherClicker.Tests;

public class UpgradeTests
{
    private Upgrade CreateTestUpgrade()
    {
        return new Upgrade("Test Upgrade", "Test Description", 100, 1.5, UpgradeType.ClickValue);
    }

    [Fact]
    public void Constructor_InitializesWithCorrectValues()
    {
        // Arrange & Act
        var upgrade = CreateTestUpgrade();

        // Assert
        Assert.Equal("Test Upgrade", upgrade.Name);
        Assert.Equal("Test Description", upgrade.Description);
        Assert.Equal(100, upgrade.BaseCost);
        Assert.Equal(1.5, upgrade.EffectValue);
        Assert.Equal(UpgradeType.ClickValue, upgrade.Type);
        Assert.Equal(0, upgrade.Level);
        Assert.False(upgrade.IsPurchased);
    }

    [Fact]
    public void CurrentCost_CalculatesCorrectly()
    {
        // Arrange
        var upgrade = CreateTestUpgrade();
        upgrade.Level = 2;
        var expectedCost = upgrade.BaseCost * System.Math.Pow(1.15, upgrade.Level);

        // Assert
        Assert.Equal(expectedCost, upgrade.CurrentCost);
    }

    [Fact]
    public void ApplyEffect_ClickValue_UpdatesGameState()
    {
        // Arrange
        var upgrade = CreateTestUpgrade();
        var gameState = new GameState();
        var initialClickValue = gameState.ClickValue;

        // Act
        upgrade.ApplyEffect(gameState);

        // Assert
        Assert.Equal(initialClickValue * upgrade.EffectValue, gameState.ClickValue);
    }

    [Fact]
    public void ApplyEffect_ProducerEfficiency_UpdatesProducer()
    {
        // Arrange
        var producer = new Producer("Test Producer", "Test Description", 100, 10);
        var upgrade = new Upgrade("Test Upgrade", "Test Description", 100, 2.0, UpgradeType.ProducerEfficiency, producer.Name);
        var gameState = new GameState();
        gameState.Producers.Add(producer);
        var initialProduction = producer.BaseProduction;

        // Act
        upgrade.ApplyEffect(gameState);

        // Assert
        Assert.Equal(initialProduction * upgrade.EffectValue, producer.BaseProduction);
    }

    [Fact]
    public void ApplyEffect_GlobalEfficiency_UpdatesGameState()
    {
        // Arrange
        var upgrade = new Upgrade("Test Upgrade", "Test Description", 100, 2.0, UpgradeType.GlobalEfficiency);
        var gameState = new GameState();
        var initialMultiplier = gameState.GlobalEfficiencyMultiplier;

        // Act
        upgrade.ApplyEffect(gameState);

        // Assert
        Assert.Equal(initialMultiplier * upgrade.EffectValue, gameState.GlobalEfficiencyMultiplier);
    }

    [Fact]
    public void ApplyEffect_CostReduction_UpdatesGameState()
    {
        // Arrange
        var upgrade = new Upgrade("Test Upgrade", "Test Description", 100, 0.8, UpgradeType.CostReduction);
        var gameState = new GameState();
        var initialMultiplier = gameState.CostReductionMultiplier;

        // Act
        upgrade.ApplyEffect(gameState);

        // Assert
        Assert.Equal(initialMultiplier * upgrade.EffectValue, gameState.CostReductionMultiplier);
    }
} 