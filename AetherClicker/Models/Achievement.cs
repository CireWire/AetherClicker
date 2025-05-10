using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Linq;

namespace AetherClicker.Models
{
    public class Achievement : INotifyPropertyChanged
    {
        private string _name;
        private string _description;
        private double _requiredValue;
        private AchievementType _type;
        private double _bonus;
        private string _targetProducerName;
        private bool _isUnlocked;
        private double _progress;
        private DateTime? _unlockTime;

        public Achievement(string name, string description, double requiredValue, AchievementType type, double bonus, string targetProducerName = "")
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _description = description ?? throw new ArgumentNullException(nameof(description));
            _requiredValue = requiredValue;
            _type = type;
            _bonus = bonus;
            _targetProducerName = targetProducerName;
            _progress = 0;
            Debug.WriteLine($"Achievement created: {name}, Type: {type}, Required: {requiredValue}");
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        public double RequiredValue
        {
            get => _requiredValue;
            set
            {
                if (_requiredValue != value)
                {
                    _requiredValue = value;
                    OnPropertyChanged();
                }
            }
        }

        public AchievementType Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Bonus
        {
            get => _bonus;
            set
            {
                if (_bonus != value)
                {
                    _bonus = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TargetProducerName
        {
            get => _targetProducerName;
            set
            {
                if (_targetProducerName != value)
                {
                    _targetProducerName = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsUnlocked
        {
            get => _isUnlocked;
            set
            {
                if (_isUnlocked != value)
                {
                    _isUnlocked = value;
                    if (value)
                    {
                        UnlockTime = DateTime.Now;
                        Debug.WriteLine($"Achievement unlocked: {Name}");
                    }
                    OnPropertyChanged();
                }
            }
        }

        public double Progress
        {
            get
            {
                if (IsUnlocked) return 1.0;
                return Math.Min(_progress / RequiredValue, 1.0);
            }
            internal set
            {
                if (_progress != value)
                {
                    _progress = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? UnlockTime
        {
            get => _unlockTime;
            set
            {
                if (_unlockTime != value)
                {
                    _unlockTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public void UpdateProgress(GameState gameState)
        {
            if (IsUnlocked) return;

            double currentValue = Type switch
            {
                AchievementType.CoinsEarned => gameState.TotalCoinsEarned,
                AchievementType.TotalProducers => gameState.Producers.Sum(p => p.Quantity),
                AchievementType.ProducerQuantity => gameState.Producers.FirstOrDefault(p => p.Name == TargetProducerName)?.Quantity ?? 0,
                AchievementType.TotalUpgrades => gameState.Upgrades.Count(u => u.IsPurchased),
                AchievementType.ClickValue => gameState.ClickValue,
                AchievementType.MagicEssence => gameState.MagicEssence,
                AchievementType.TimePlayed => gameState.TimePlayed.TotalSeconds,
                _ => 0
            };

            Progress = currentValue;
            Debug.WriteLine($"Achievement progress updated: {Name}, Current: {currentValue}, Required: {RequiredValue}, Progress: {Progress}");
            
            if (currentValue >= RequiredValue)
            {
                IsUnlocked = true;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum AchievementType
    {
        CoinsEarned,         // Total coins earned
        TotalProducers,      // Total number of producers owned
        ProducerQuantity,    // Number of a specific producer owned
        TotalUpgrades,       // Total number of upgrades purchased
        ClickValue,          // Click value milestone
        MagicEssence,        // Magic essence milestone
        TimePlayed          // Time spent playing
    }
} 