using AetherClicker.Models;
using AetherClicker.Utils;
using System.IO;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace AetherClicker.Tests;

public class SaveLoadPrestigeTests : IDisposable
{
    private readonly string _savePath;
    private readonly GameState _gameState;

    public SaveLoadPrestigeTests()
    {
        _savePath = Path.Combine(Path.GetTempPath(), $"test_save_{Guid.NewGuid()}.json");
        _gameState = CreateTestGameState();
    }

    public void Dispose()
    {
        if (File.Exists(_savePath))
        {
            File.Delete(_savePath);
        }
    }

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

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void SaveGame_AfterPrestige_PreservesPrestigeLevel(int prestigeCount)
    {
        // Arrange
        for (int i = 0; i < prestigeCount; i++)
        {
            _gameState.AddCoins(1_000_000);
            _gameState.PerformPrestige();
            if (i < prestigeCount - 1)
            {
                _gameState.AddCoins(10000);
                _gameState.AddMagicEssence(5000);
            }
        }

        // Act
        var saveData = new SaveData
        {
            Coins = _gameState.Coins,
            MagicEssence = _gameState.MagicEssence,
            ClickValue = _gameState.ClickValue,
            PlayerName = _gameState.PlayerName,
            CompanyName = _gameState.CompanyName,
            TimePlayed = _gameState.TimePlayed,
            TotalCoinsEarned = _gameState.TotalCoinsEarned,
            GlobalEfficiencyMultiplier = _gameState.GlobalEfficiencyMultiplier,
            CostReductionMultiplier = _gameState.CostReductionMultiplier,
            SelectedBackground = _gameState.SelectedBackground,
            SelectedSpecialization = _gameState.SelectedSpecialization,
            PrestigeLevel = _gameState.PrestigeLevel
        };
        SaveManager.SaveGame(saveData, _savePath);
        var loadedGameState = new GameState();
        loadedGameState.LoadFromSaveData(SaveManager.LoadGame(_savePath));

        // Assert
        Assert.Equal(prestigeCount, loadedGameState.PrestigeLevel);
        var expectedMultiplier = 1.0 + (prestigeCount * 0.1);
        Assert.Equal(expectedMultiplier, loadedGameState.PrestigeMultiplier);
    }

    [Fact]
    public void LoadGame_AfterPrestige_MaintainsPrestigeBonus()
    {
        // Arrange
        _gameState.AddCoins(1_000_000);
        _gameState.PerformPrestige();
        var saveData = new SaveData
        {
            Coins = _gameState.Coins,
            MagicEssence = _gameState.MagicEssence,
            ClickValue = _gameState.ClickValue,
            PlayerName = _gameState.PlayerName,
            CompanyName = _gameState.CompanyName,
            TimePlayed = _gameState.TimePlayed,
            TotalCoinsEarned = _gameState.TotalCoinsEarned,
            GlobalEfficiencyMultiplier = _gameState.GlobalEfficiencyMultiplier,
            CostReductionMultiplier = _gameState.CostReductionMultiplier,
            SelectedBackground = _gameState.SelectedBackground,
            SelectedSpecialization = _gameState.SelectedSpecialization,
            PrestigeLevel = _gameState.PrestigeLevel
        };
        SaveManager.SaveGame(saveData, _savePath);
        var loadedGameState = new GameState();
        loadedGameState.LoadFromSaveData(SaveManager.LoadGame(_savePath));

        // Act
        var producer = new Producer("Test Producer", "Test Description", 100, 10);
        loadedGameState.AddProducer(producer);
        producer.Purchase();

        // Assert
        var expectedProduction = 10 * loadedGameState.PrestigeMultiplier;
        Assert.Equal(expectedProduction, producer.CalculateProduction());
    }

    [Fact]
    public void SaveLoad_WithMultiplePrestiges_PreservesAllPrestigeLevels()
    {
        // Arrange
        _gameState.AddCoins(1_000_000);
        _gameState.PerformPrestige(); // Level 1
        _gameState.AddCoins(1_000_000);
        _gameState.PerformPrestige(); // Level 2
        var saveData = new SaveData
        {
            Coins = _gameState.Coins,
            MagicEssence = _gameState.MagicEssence,
            ClickValue = _gameState.ClickValue,
            PlayerName = _gameState.PlayerName,
            CompanyName = _gameState.CompanyName,
            TimePlayed = _gameState.TimePlayed,
            TotalCoinsEarned = _gameState.TotalCoinsEarned,
            GlobalEfficiencyMultiplier = _gameState.GlobalEfficiencyMultiplier,
            CostReductionMultiplier = _gameState.CostReductionMultiplier,
            SelectedBackground = _gameState.SelectedBackground,
            SelectedSpecialization = _gameState.SelectedSpecialization,
            PrestigeLevel = _gameState.PrestigeLevel
        };
        SaveManager.SaveGame(saveData, _savePath);

        // Act
        var loadedGameState = new GameState();
        loadedGameState.LoadFromSaveData(SaveManager.LoadGame(_savePath));

        // Assert
        Assert.Equal(2, loadedGameState.PrestigeLevel);
        var expectedMultiplier = 1.0 + (loadedGameState.PrestigeLevel * 0.1);
        Assert.Equal(expectedMultiplier, loadedGameState.PrestigeMultiplier);
    }

    [Fact]
    public void SaveLoad_AfterPrestige_MaintainsAchievements()
    {
        // Arrange
        _gameState.AddCoins(1_000_000);
        _gameState.PerformPrestige();
        var saveData = new SaveData
        {
            Coins = _gameState.Coins,
            MagicEssence = _gameState.MagicEssence,
            ClickValue = _gameState.ClickValue,
            PlayerName = _gameState.PlayerName,
            CompanyName = _gameState.CompanyName,
            TimePlayed = _gameState.TimePlayed,
            TotalCoinsEarned = _gameState.TotalCoinsEarned,
            GlobalEfficiencyMultiplier = _gameState.GlobalEfficiencyMultiplier,
            CostReductionMultiplier = _gameState.CostReductionMultiplier,
            SelectedBackground = _gameState.SelectedBackground,
            SelectedSpecialization = _gameState.SelectedSpecialization,
            PrestigeLevel = _gameState.PrestigeLevel,
            Achievements = _gameState.Achievements.Select(a => new AchievementSaveData
            {
                Name = a.Name,
                Description = a.Description,
                RequiredValue = a.RequiredValue,
                Type = a.Type,
                Bonus = a.Bonus,
                IsUnlocked = a.IsUnlocked,
                TargetProducerName = a.TargetProducerName
            }).ToList()
        };
        SaveManager.SaveGame(saveData, _savePath);

        // Act
        var loadedGameState = new GameState();
        loadedGameState.LoadFromSaveData(SaveManager.LoadGame(_savePath));

        // Assert
        Assert.NotEmpty(loadedGameState.Achievements);
        Assert.All(loadedGameState.Achievements, a => Assert.True(a.IsUnlocked));
    }

    [Fact]
    public void SaveLoad_AfterPrestige_ResetsGameElements()
    {
        // Arrange
        _gameState.AddCoins(1_000_000);
        _gameState.PerformPrestige();
        var saveData = new SaveData
        {
            Coins = _gameState.Coins,
            MagicEssence = _gameState.MagicEssence,
            ClickValue = _gameState.ClickValue,
            PlayerName = _gameState.PlayerName,
            CompanyName = _gameState.CompanyName,
            TimePlayed = _gameState.TimePlayed,
            TotalCoinsEarned = _gameState.TotalCoinsEarned,
            GlobalEfficiencyMultiplier = _gameState.GlobalEfficiencyMultiplier,
            CostReductionMultiplier = _gameState.CostReductionMultiplier,
            SelectedBackground = _gameState.SelectedBackground,
            SelectedSpecialization = _gameState.SelectedSpecialization,
            PrestigeLevel = _gameState.PrestigeLevel
        };
        SaveManager.SaveGame(saveData, _savePath);

        // Act
        var loadedGameState = new GameState();
        loadedGameState.LoadFromSaveData(SaveManager.LoadGame(_savePath));

        // Assert
        Assert.Empty(loadedGameState.Producers);
        Assert.Empty(loadedGameState.Upgrades);
        Assert.Empty(loadedGameState.Enhancements);
    }

    [Fact]
    public void SaveLoad_AfterPrestige_MaintainsCustomization()
    {
        // Arrange
        _gameState.AddCoins(1_000_000);
        _gameState.PerformPrestige();
        var saveData = new SaveData
        {
            Coins = _gameState.Coins,
            MagicEssence = _gameState.MagicEssence,
            ClickValue = _gameState.ClickValue,
            PlayerName = _gameState.PlayerName,
            CompanyName = _gameState.CompanyName,
            TimePlayed = _gameState.TimePlayed,
            TotalCoinsEarned = _gameState.TotalCoinsEarned,
            GlobalEfficiencyMultiplier = _gameState.GlobalEfficiencyMultiplier,
            CostReductionMultiplier = _gameState.CostReductionMultiplier,
            SelectedBackground = _gameState.SelectedBackground,
            SelectedSpecialization = _gameState.SelectedSpecialization,
            PrestigeLevel = _gameState.PrestigeLevel
        };
        SaveManager.SaveGame(saveData, _savePath);

        // Act
        var loadedGameState = new GameState();
        loadedGameState.LoadFromSaveData(SaveManager.LoadGame(_savePath));

        // Assert
        Assert.Equal(_gameState.PlayerName, loadedGameState.PlayerName);
        Assert.Equal(_gameState.CompanyName, loadedGameState.CompanyName);
        Assert.Equal(_gameState.SelectedBackground, loadedGameState.SelectedBackground);
        Assert.Equal(_gameState.SelectedSpecialization, loadedGameState.SelectedSpecialization);
    }

    [Fact]
    public void SaveLoad_WithCorruptedFile_ThrowsException()
    {
        // Arrange
        File.WriteAllText(_savePath, "invalid json");

        // Act & Assert
        Assert.Throws<Exception>(() => SaveManager.LoadGame(_savePath));
    }

    [Fact]
    public void SaveLoad_WithEmptyFile_ThrowsException()
    {
        // Arrange
        File.WriteAllText(_savePath, "");

        // Act & Assert
        Assert.Throws<Exception>(() => SaveManager.LoadGame(_savePath));
    }

    [Fact]
    public async Task SaveLoad_WithConcurrentAccess_HandlesCorrectly()
    {
        // Arrange
        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                var saveData = new SaveData
                {
                    Coins = 1000,
                    MagicEssence = 500,
                    ClickValue = 1,
                    PlayerName = "TestPlayer",
                    CompanyName = "TestCompany",
                    TimePlayed = TimeSpan.Zero,
                    TotalCoinsEarned = 1000,
                    GlobalEfficiencyMultiplier = 1.0,
                    CostReductionMultiplier = 1.0,
                    SelectedBackground = "Mystic",
                    SelectedSpecialization = "Alchemy",
                    PrestigeLevel = 0
                };
                SaveManager.SaveGame(saveData, _savePath);
                var loadedData = SaveManager.LoadGame(_savePath);
                Assert.NotNull(loadedData);
            }));
        }

        // Act & Assert
        await Task.WhenAll(tasks);
    }

    [Fact]
    public void SaveLoad_WithLargeGameState_HandlesCorrectly()
    {
        // Arrange
        var saveData = new SaveData
        {
            Coins = 1_000_000_000,
            MagicEssence = 500_000_000,
            ClickValue = 1000,
            PlayerName = "TestPlayer",
            CompanyName = "TestCompany",
            TimePlayed = TimeSpan.FromHours(1000),
            TotalCoinsEarned = 1_000_000_000,
            GlobalEfficiencyMultiplier = 10.0,
            CostReductionMultiplier = 0.5,
            SelectedBackground = "Mystic",
            SelectedSpecialization = "Alchemy",
            PrestigeLevel = 10,
            Achievements = Enumerable.Range(1, 100).Select(i => new AchievementSaveData
            {
                Name = $"Achievement {i}",
                Description = $"Description {i}",
                RequiredValue = i * 1000,
                Type = AchievementType.CoinsEarned,
                Bonus = 1.1,
                IsUnlocked = true,
                TargetProducerName = "producer"
            }).ToList()
        };

        // Act
        SaveManager.SaveGame(saveData, _savePath);
        var loadedData = SaveManager.LoadGame(_savePath);

        // Assert
        Assert.NotNull(loadedData);
        Assert.Equal(saveData.Coins, loadedData.Coins);
        Assert.Equal(saveData.MagicEssence, loadedData.MagicEssence);
        Assert.Equal(saveData.PrestigeLevel, loadedData.PrestigeLevel);
        Assert.Equal(saveData.Achievements.Count, loadedData.Achievements.Count);
    }
} 