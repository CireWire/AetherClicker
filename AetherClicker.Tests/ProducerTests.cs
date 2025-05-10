using AetherClicker.Models;
using Xunit;

namespace AetherClicker.Tests;

public class ProducerTests
{
    private Producer CreateTestProducer()
    {
        return new Producer("Test Producer", "Test Description", 100, 10);
    }

    [Fact]
    public void Constructor_InitializesWithCorrectValues()
    {
        // Arrange & Act
        var producer = CreateTestProducer();

        // Assert
        Assert.Equal("Test Producer", producer.Name);
        Assert.Equal("Test Description", producer.Description);
        Assert.Equal(100, producer.BaseCost);
        Assert.Equal(10, producer.BaseProduction);
        Assert.Equal(0, producer.Quantity);
        Assert.Empty(producer.Enhancements);
    }

    [Fact]
    public void CurrentCost_CalculatesCorrectly()
    {
        // Arrange
        var producer = CreateTestProducer();
        producer.Quantity = 2;
        var expectedCost = producer.BaseCost * System.Math.Pow(1.15, producer.Quantity);

        // Assert
        Assert.Equal(expectedCost, producer.CurrentCost);
    }

    [Fact]
    public void CurrentProduction_CalculatesCorrectly()
    {
        // Arrange
        var producer = CreateTestProducer();
        producer.Quantity = 2;
        var expectedProduction = producer.BaseProduction * producer.Quantity;

        // Assert
        Assert.Equal(expectedProduction, producer.CurrentProduction);
    }

    [Fact]
    public void ApplyEnhancement_AddsEnhancementToList()
    {
        // Arrange
        var producer = CreateTestProducer();
        var enhancement = new Enhancement("Test Enhancement", "Test Description", 100, 1.5, EnhancementType.Efficiency);

        // Act
        producer.ApplyEnhancement(enhancement);

        // Assert
        Assert.Single(producer.Enhancements);
        Assert.Contains(enhancement, producer.Enhancements);
    }

    [Fact]
    public void CurrentProduction_WithEfficiencyEnhancement_CalculatesCorrectly()
    {
        // Arrange
        var producer = CreateTestProducer();
        producer.Quantity = 2;
        var enhancement = new Enhancement("Test Enhancement", "Test Description", 100, 1.5, EnhancementType.Efficiency);
        producer.ApplyEnhancement(enhancement);
        var expectedProduction = producer.BaseProduction * producer.Quantity * enhancement.EffectValue;

        // Assert
        Assert.Equal(expectedProduction, producer.CurrentProduction);
    }

    [Fact]
    public void CurrentCost_WithCostReductionEnhancement_CalculatesCorrectly()
    {
        // Arrange
        var producer = CreateTestProducer();
        producer.Quantity = 2;
        var enhancement = new Enhancement("Test Enhancement", "Test Description", 100, 0.8, EnhancementType.CostReduction);
        producer.ApplyEnhancement(enhancement);
        var expectedCost = producer.BaseCost * System.Math.Pow(1.15, producer.Quantity) * enhancement.EffectValue;

        // Assert
        Assert.Equal(expectedCost, producer.CurrentCost);
    }
} 