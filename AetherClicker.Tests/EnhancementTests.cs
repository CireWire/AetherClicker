using AetherClicker.Models;
using Xunit;

namespace AetherClicker.Tests;

public class EnhancementTests
{
    private Enhancement CreateTestEnhancement()
    {
        return new Enhancement("Test Enhancement", "Test Description", 100, 1.5, EnhancementType.Efficiency);
    }

    [Fact]
    public void Constructor_InitializesWithCorrectValues()
    {
        // Arrange & Act
        var enhancement = CreateTestEnhancement();

        // Assert
        Assert.Equal("Test Enhancement", enhancement.Name);
        Assert.Equal("Test Description", enhancement.Description);
        Assert.Equal(100, enhancement.BaseCost);
        Assert.Equal(1.5, enhancement.EffectValue);
        Assert.Equal(EnhancementType.Efficiency, enhancement.Type);
    }

    [Fact]
    public void ApplyToProducer_AddsEnhancementToProducer()
    {
        // Arrange
        var enhancement = CreateTestEnhancement();
        var producer = new Producer("Test Producer", "Test Description", 100, 10);

        // Act
        producer.ApplyEnhancement(enhancement);

        // Assert
        Assert.Contains(enhancement, producer.Enhancements);
    }

    [Fact]
    public void ApplyToProducer_WithEfficiencyType_IncreasesProduction()
    {
        // Arrange
        var enhancement = CreateTestEnhancement();
        var producer = new Producer("Test Producer", "Test Description", 100, 10);
        var initialProduction = producer.CurrentProduction;

        // Act
        producer.ApplyEnhancement(enhancement);

        // Assert
        Assert.Equal(initialProduction * enhancement.EffectValue, producer.CurrentProduction);
    }

    [Fact]
    public void ApplyToProducer_WithCostReductionType_ReducesCost()
    {
        // Arrange
        var enhancement = new Enhancement("Test Enhancement", "Test Description", 100, 0.8, EnhancementType.CostReduction);
        var producer = new Producer("Test Producer", "Test Description", 100, 10);
        var initialCost = producer.CurrentCost;

        // Act
        producer.ApplyEnhancement(enhancement);

        // Assert
        Assert.Equal(initialCost * enhancement.EffectValue, producer.CurrentCost);
    }

    [Fact]
    public void ApplyToProducer_WithQuantityBonus_IncreasesProduction()
    {
        // Arrange
        var enhancement = new Enhancement("Test Enhancement", "Test Description", 100, 2.0, EnhancementType.QuantityBonus);
        var producer = new Producer("Test Producer", "Test Description", 100, 10);
        producer.Quantity = 1;
        var initialProduction = producer.CurrentProduction;

        // Act
        producer.ApplyEnhancement(enhancement);

        // Assert
        Assert.Equal(initialProduction * enhancement.EffectValue, producer.CurrentProduction);
    }
} 