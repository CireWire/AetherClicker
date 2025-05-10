using AetherClicker.Models;
using AetherClicker.Utils;
using System.IO;
using Xunit;
using System;

namespace AetherClicker.Tests;

public class SaveLoadTests : IDisposable
{
    private readonly string _savePath;
    private readonly GameState _gameState;

    public SaveLoadTests()
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

        var upgrade = new Upgrade("Test Upgrade", "Test Description", 200, 2.0, UpgradeType.GlobalEfficiency);
        upgrade.Purchase();
        gameState.AddUpgrade(upgrade);

        var achievement = new Achievement("Test Achievement", "Test Description", 100, AchievementType.CoinsEarned, 1.1);
        achievement.IsUnlocked = true;
        gameState.AddAchievement(achievement);

        var enhancement = new Enhancement("Test Enhancement", "Test Description", 300, 2.0, EnhancementType.Efficiency);
        enhancement.Purchase();
        gameState.AddEnhancement(enhancement);

        return gameState;
    }

    [Fact]
    public void SaveLoad_PreservesGameState()
    {
        // Arrange
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

        // Act
        SaveManager.SaveGame(saveData, _savePath);
        var loadedGameState = new GameState();
        loadedGameState.LoadFromSaveData(SaveManager.LoadGame(_savePath));

        // Assert
        Assert.Equal(_gameState.Coins, loadedGameState.Coins);
        Assert.Equal(_gameState.MagicEssence, loadedGameState.MagicEssence);
        Assert.Equal(_gameState.PlayerName, loadedGameState.PlayerName);
        Assert.Equal(_gameState.CompanyName, loadedGameState.CompanyName);
        Assert.Equal(_gameState.SelectedBackground, loadedGameState.SelectedBackground);
        Assert.Equal(_gameState.SelectedSpecialization, loadedGameState.SelectedSpecialization);
    }

    [Fact]
    public void SaveLoad_PreservesGameElements()
    {
        // Arrange
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
            Producers = _gameState.Producers.Select(p => new ProducerSaveData
            {
                Name = p.Name,
                Description = p.Description,
                Quantity = p.Quantity,
                BaseCost = p.BaseCost,
                BaseProduction = p.BaseProduction,
                EfficiencyMultiplier = p.EfficiencyMultiplier,
                CostReductionMultiplier = p.CostReductionMultiplier,
                QuantityMultiplier = p.QuantityMultiplier,
                EnhancementIds = p.Enhancements.Select(e => e.Name).ToList()
            }).ToList(),
            Upgrades = _gameState.Upgrades.Select(u => new UpgradeSaveData
            {
                Name = u.Name,
                Description = u.Description,
                Cost = u.BaseCost,
                Effect = u.EffectValue,
                Type = u.Type,
                TargetProducerName = u.TargetProducerName,
                IsPurchased = u.IsPurchased
            }).ToList(),
            Achievements = _gameState.Achievements.Select(a => new AchievementSaveData
            {
                Name = a.Name,
                Description = a.Description,
                RequiredValue = a.RequiredValue,
                Type = a.Type,
                Bonus = a.Bonus,
                TargetProducerName = a.TargetProducerName,
                IsUnlocked = a.IsUnlocked,
                Progress = a.Progress,
                UnlockTime = a.UnlockTime
            }).ToList(),
            Enhancements = _gameState.Enhancements.Select(e => new EnhancementSaveData
            {
                Name = e.Name,
                Description = e.Description,
                Cost = e.Cost,
                Effect = e.Effect,
                Type = e.Type,
                IsPurchased = e.IsPurchased,
                IsActive = e.IsActive
            }).ToList()
        };

        // Act
        SaveManager.SaveGame(saveData, _savePath);
        var loadedGameState = new GameState();
        loadedGameState.LoadFromSaveData(SaveManager.LoadGame(_savePath));

        // Assert
        Assert.Single(loadedGameState.Producers);
        Assert.Single(loadedGameState.Upgrades);
        Assert.Single(loadedGameState.Achievements);
        Assert.Single(loadedGameState.Enhancements);

        var producer = loadedGameState.Producers.First();
        Assert.Equal("Test Producer", producer.Name);
        Assert.Equal(5, producer.Quantity);

        var upgrade = loadedGameState.Upgrades.First();
        Assert.Equal("Test Upgrade", upgrade.Name);
        Assert.True(upgrade.IsPurchased);

        var achievement = loadedGameState.Achievements.First();
        Assert.Equal("Test Achievement", achievement.Name);
        Assert.True(achievement.IsUnlocked);

        var enhancement = loadedGameState.Enhancements.First();
        Assert.Equal("Test Enhancement", enhancement.Name);
        Assert.True(enhancement.IsPurchased);
    }

    [Fact]
    public void SaveLoad_WithCorruptedFile_ThrowsException()
    {
        // Arrange
        File.WriteAllText(_savePath, "invalid json content");

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => SaveManager.LoadGame(_savePath));
    }

    [Fact]
    public void SaveLoad_WithEmptyFile_ThrowsException()
    {
        // Arrange
        File.WriteAllText(_savePath, string.Empty);

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => SaveManager.LoadGame(_savePath));
    }
} 