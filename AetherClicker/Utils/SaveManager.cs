using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using AetherClicker.Models;
using System.Diagnostics;
using System.Linq;

namespace AetherClicker.Utils
{
    public static class SaveManager
    {
        private static readonly string DefaultSavePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AetherClicker",
            "save.json"
        );

        public static void SaveGame(SaveData saveData, string? savePath = null)
        {
            savePath ??= DefaultSavePath;
            var directory = Path.GetDirectoryName(savePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string jsonString = JsonSerializer.Serialize(saveData, options);
            File.WriteAllText(savePath, jsonString);
        }

        public static SaveData LoadGame(string? savePath = null)
        {
            savePath ??= DefaultSavePath;
            if (!File.Exists(savePath))
            {
                return new SaveData();
            }

            string jsonString = File.ReadAllText(savePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<SaveData>(jsonString, options) ?? new SaveData();
        }

        public static async Task<bool> SaveGameAsync(GameState gameState)
        {
            try
            {
                // Create save directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(DefaultSavePath) ?? string.Empty);

                var saveData = new SaveData
                {
                    Coins = gameState.Coins,
                    MagicEssence = gameState.MagicEssence,
                    ClickValue = gameState.ClickValue,
                    PlayerName = gameState.PlayerName,
                    CompanyName = gameState.CompanyName,
                    GlobalEfficiencyMultiplier = gameState.GlobalEfficiencyMultiplier,
                    CostReductionMultiplier = gameState.CostReductionMultiplier,
                    TotalCoinsEarned = gameState.TotalCoinsEarned,
                    TimePlayed = gameState.TimePlayed,
                    PrestigeLevel = gameState.PrestigeLevel,
                    SelectedBackground = gameState.SelectedBackground,
                    SelectedSpecialization = gameState.SelectedSpecialization
                };

                // Save producers
                foreach (var producer in gameState.Producers)
                {
                    saveData.Producers.Add(new ProducerSaveData
                    {
                        Name = producer.Name,
                        Description = producer.Description,
                        Quantity = producer.Quantity,
                        BaseCost = producer.BaseCost,
                        BaseProduction = producer.BaseProduction,
                        EfficiencyMultiplier = producer._efficiencyMultiplier,
                        CostReductionMultiplier = producer._costReductionMultiplier,
                        QuantityMultiplier = producer._quantityMultiplier,
                        EnhancementIds = producer.Enhancements.Select(e => e.Name).ToList()
                    });
                }

                // Save upgrades
                foreach (var upgrade in gameState.Upgrades)
                {
                    saveData.Upgrades.Add(new UpgradeSaveData
                    {
                        Name = upgrade.Name,
                        Description = upgrade.Description,
                        Cost = upgrade.BaseCost,
                        Effect = upgrade.EffectValue,
                        Type = upgrade.Type,
                        TargetProducerName = upgrade.TargetProducerName,
                        IsPurchased = upgrade.IsPurchased
                    });
                }

                // Save achievements
                foreach (var achievement in gameState.Achievements)
                {
                    saveData.Achievements.Add(new AchievementSaveData
                    {
                        Name = achievement.Name,
                        Description = achievement.Description,
                        RequiredValue = achievement.RequiredValue,
                        Type = achievement.Type,
                        Bonus = achievement.Bonus,
                        TargetProducerName = achievement.TargetProducerName,
                        IsUnlocked = achievement.IsUnlocked,
                        Progress = achievement.Progress,
                        UnlockTime = achievement.UnlockTime
                    });
                }

                // Save enhancements
                foreach (var enhancement in gameState.Enhancements)
                {
                    saveData.Enhancements.Add(new EnhancementSaveData
                    {
                        Name = enhancement.Name,
                        Description = enhancement.Description,
                        Cost = enhancement.Cost,
                        Effect = enhancement.Effect,
                        Type = enhancement.Type,
                        IsPurchased = enhancement.IsPurchased,
                        IsActive = enhancement.IsActive
                    });
                }

                // Serialize to JSON
                string json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                // Save to file
                string savePath = DefaultSavePath;
                await File.WriteAllTextAsync(savePath, json);

                Debug.WriteLine($"Game saved successfully to {savePath}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving game: {ex.Message}");
                return false;
            }
        }

        public static async Task<SaveData?> LoadGameAsync()
        {
            try
            {
                string savePath = DefaultSavePath;
                if (!File.Exists(savePath))
                {
                    Debug.WriteLine("No save file found");
                    return null;
                }

                string json = await File.ReadAllTextAsync(savePath);
                var saveData = JsonSerializer.Deserialize<SaveData>(json);

                Debug.WriteLine($"Game loaded successfully from {savePath}");
                return saveData;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading game: {ex.Message}");
                return null;
            }
        }
    }
} 