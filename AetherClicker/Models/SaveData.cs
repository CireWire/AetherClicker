using System;
using System.Collections.Generic;

namespace AetherClicker.Models
{
    public class SaveData
    {
        public double Coins { get; set; }
        public double MagicEssence { get; set; }
        public double ClickValue { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public TimeSpan TimePlayed { get; set; }
        public double TotalCoinsEarned { get; set; }
        public double GlobalEfficiencyMultiplier { get; set; }
        public double CostReductionMultiplier { get; set; }
        public string SelectedBackground { get; set; } = string.Empty;
        public string SelectedSpecialization { get; set; } = string.Empty;
        public int PrestigeLevel { get; set; }
        public List<ProducerSaveData> Producers { get; set; } = new();
        public List<UpgradeSaveData> Upgrades { get; set; } = new();
        public List<AchievementSaveData> Achievements { get; set; } = new();
        public List<EnhancementSaveData> Enhancements { get; set; } = new();
    }

    public class ProducerSaveData
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double BaseCost { get; set; }
        public double BaseProduction { get; set; }
        public double EfficiencyMultiplier { get; set; }
        public double CostReductionMultiplier { get; set; }
        public double QuantityMultiplier { get; set; }
        public List<string> EnhancementIds { get; set; } = new();
    }

    public class UpgradeSaveData
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Cost { get; set; }
        public double Effect { get; set; }
        public UpgradeType Type { get; set; }
        public string? TargetProducerName { get; set; }
        public bool IsPurchased { get; set; }
    }

    public class AchievementSaveData
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double RequiredValue { get; set; }
        public AchievementType Type { get; set; }
        public double Bonus { get; set; }
        public string? TargetProducerName { get; set; }
        public bool IsUnlocked { get; set; }
        public double Progress { get; set; }
        public DateTime? UnlockTime { get; set; }
    }

    public class EnhancementSaveData
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Cost { get; set; }
        public double Effect { get; set; }
        public EnhancementType Type { get; set; }
        public bool IsPurchased { get; set; }
        public bool IsActive { get; set; }
    }
} 