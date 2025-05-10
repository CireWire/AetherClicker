using AetherClicker.Models;
using Xunit;

namespace AetherClicker.Tests;

public class AchievementTests
{
    private Achievement CreateTestAchievement()
    {
        return new Achievement("Test Achievement", "Test Description", 100, AchievementType.CoinsEarned, 1.5, "producer");
    }

    [Fact]
    public void Constructor_InitializesWithCorrectValues()
    {
        // Arrange & Act
        var achievement = CreateTestAchievement();

        // Assert
        Assert.Equal("Test Achievement", achievement.Name);
        Assert.Equal("Test Description", achievement.Description);
        Assert.Equal(100, achievement.RequiredValue);
        Assert.Equal(AchievementType.CoinsEarned, achievement.Type);
        Assert.Equal(1.5, achievement.Bonus);
        Assert.False(achievement.IsUnlocked);
    }

    [Fact]
    public void CheckProgress_WithSufficientProgress_UnlocksAchievement()
    {
        // Arrange
        var achievement = CreateTestAchievement();
        var gameState = new GameState();
        gameState.AddCoins(200); // More than required value

        // Act
        achievement.UpdateProgress(gameState);

        // Assert
        Assert.True(achievement.IsUnlocked);
    }

    [Fact]
    public void CheckProgress_WithInsufficientProgress_DoesNotUnlockAchievement()
    {
        // Arrange
        var achievement = CreateTestAchievement();
        var gameState = new GameState();
        gameState.AddCoins(50); // Less than required value

        // Act
        achievement.UpdateProgress(gameState);

        // Assert
        Assert.False(achievement.IsUnlocked);
    }

    [Fact]
    public void GetProgress_ReturnsCorrectPercentage()
    {
        // Arrange
        var achievement = CreateTestAchievement();
        var gameState = new GameState();
        gameState.AddCoins(50); // 50% of required value

        // Act
        var progress = achievement.GetProgress(gameState);

        // Assert
        Assert.Equal(0.5, progress);
    }

    [Fact]
    public void GetProgress_WhenUnlocked_ReturnsOne()
    {
        // Arrange
        var achievement = CreateTestAchievement();
        var gameState = new GameState();
        gameState.AddCoins(200); // More than required value
        achievement.UpdateProgress(gameState);

        // Act
        var progress = achievement.GetProgress(gameState);

        // Assert
        Assert.Equal(1.0, progress);
    }
} 