using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace AetherClicker.Models
{
    public class Prestige : INotifyPropertyChanged
    {
        private int _prestigeLevel;
        private double _prestigePoints;
        private double _prestigeMultiplier;
        private double _magicEssenceMultiplier;
        private double _coinMultiplier;
        private double _clickValueMultiplier;
        private double _producerEfficiencyMultiplier;
        private double _upgradeCostReductionMultiplier;

        public Prestige()
        {
            _prestigeLevel = 0;
            _prestigePoints = 0;
            _prestigeMultiplier = 1.0;
            _magicEssenceMultiplier = 1.0;
            _coinMultiplier = 1.0;
            _clickValueMultiplier = 1.0;
            _producerEfficiencyMultiplier = 1.0;
            _upgradeCostReductionMultiplier = 1.0;
            Debug.WriteLine("Prestige system initialized");
        }

        public int PrestigeLevel
        {
            get => _prestigeLevel;
            set
            {
                if (_prestigeLevel != value)
                {
                    _prestigeLevel = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PrestigeCost));
                    Debug.WriteLine($"Prestige level changed to: {value}");
                }
            }
        }

        public double PrestigePoints
        {
            get => _prestigePoints;
            set
            {
                if (_prestigePoints != value)
                {
                    _prestigePoints = value;
                    OnPropertyChanged();
                    Debug.WriteLine($"Prestige points changed to: {value}");
                }
            }
        }

        public double PrestigeMultiplier
        {
            get => _prestigeMultiplier;
            set
            {
                if (_prestigeMultiplier != value)
                {
                    _prestigeMultiplier = value;
                    OnPropertyChanged();
                    Debug.WriteLine($"Prestige multiplier changed to: {value}");
                }
            }
        }

        public double PrestigeCost => 1000000 * Math.Pow(10, PrestigeLevel);

        public double MagicEssenceMultiplier
        {
            get => _magicEssenceMultiplier;
            set
            {
                if (_magicEssenceMultiplier != value)
                {
                    _magicEssenceMultiplier = value;
                    OnPropertyChanged();
                    Debug.WriteLine($"Magic essence multiplier changed to: {value}");
                }
            }
        }

        public double CoinMultiplier
        {
            get => _coinMultiplier;
            set
            {
                if (_coinMultiplier != value)
                {
                    _coinMultiplier = value;
                    OnPropertyChanged();
                    Debug.WriteLine($"Coin multiplier changed to: {value}");
                }
            }
        }

        public double ClickValueMultiplier
        {
            get => _clickValueMultiplier;
            set
            {
                if (_clickValueMultiplier != value)
                {
                    _clickValueMultiplier = value;
                    OnPropertyChanged();
                    Debug.WriteLine($"Click value multiplier changed to: {value}");
                }
            }
        }

        public double ProducerEfficiencyMultiplier
        {
            get => _producerEfficiencyMultiplier;
            set
            {
                if (_producerEfficiencyMultiplier != value)
                {
                    _producerEfficiencyMultiplier = value;
                    OnPropertyChanged();
                    Debug.WriteLine($"Producer efficiency multiplier changed to: {value}");
                }
            }
        }

        public double UpgradeCostReductionMultiplier
        {
            get => _upgradeCostReductionMultiplier;
            set
            {
                if (_upgradeCostReductionMultiplier != value)
                {
                    _upgradeCostReductionMultiplier = value;
                    OnPropertyChanged();
                    Debug.WriteLine($"Upgrade cost reduction multiplier changed to: {value}");
                }
            }
        }

        public bool CanPrestige(double totalCoinsEarned, int totalProducers, int totalUpgrades)
        {
            return CalculatePrestigePoints(totalCoinsEarned, totalProducers, totalUpgrades) > 0;
        }

        public double CalculatePrestigePoints(double totalCoinsEarned, int totalProducers, int totalUpgrades)
        {
            return Math.Floor((totalCoinsEarned / 1000000) + (totalProducers * 0.1) + (totalUpgrades * 0.2));
        }

        public void ApplyPrestige()
        {
            PrestigeLevel++;
            PrestigeMultiplier = 1 + (PrestigeLevel * 0.1);
            MagicEssenceMultiplier *= 1.2; // 20% increase in magic essence gain
            CoinMultiplier *= 1.15; // 15% increase in coin gain
            ClickValueMultiplier *= 1.1; // 10% increase in click value
            ProducerEfficiencyMultiplier *= 1.1; // 10% increase in producer efficiency
            UpgradeCostReductionMultiplier *= 0.95; // 5% reduction in upgrade costs

            Debug.WriteLine($"Prestige applied - New level: {PrestigeLevel}");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
        }
    }
} 