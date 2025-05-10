using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Linq;

namespace AetherClicker.Models
{
    public class Upgrade : INotifyPropertyChanged
    {
        private string _name;
        private string _description;
        private double _baseCost;
        private double _effectValue;
        private UpgradeType _type;
        private string _targetProducerName;
        private bool _isPurchased;
        private int _level;

        public Upgrade(string name, string description, double baseCost, double effectValue, UpgradeType type, string targetProducerName = "")
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _description = description ?? throw new ArgumentNullException(nameof(description));
            _baseCost = baseCost;
            _effectValue = effectValue;
            _type = type;
            _targetProducerName = targetProducerName;
            _isPurchased = false;
            Debug.WriteLine($"Upgrade created: {name}, Effect: {effectValue}");
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

        public double BaseCost
        {
            get => _baseCost;
            set
            {
                if (_baseCost != value)
                {
                    _baseCost = value;
                    OnPropertyChanged();
                }
            }
        }

        public double CurrentCost => BaseCost * Math.Pow(1.15, Level);

        public double EffectValue
        {
            get => _effectValue;
            set
            {
                if (_effectValue != value)
                {
                    _effectValue = value;
                    OnPropertyChanged();
                }
            }
        }

        public UpgradeType Type
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

        public bool IsPurchased
        {
            get => _isPurchased;
            set
            {
                if (_isPurchased != value)
                {
                    _isPurchased = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Level
        {
            get => _level;
            set
            {
                _level = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentCost));
            }
        }

        public double Cost
        {
            get => BaseCost;
            set { BaseCost = value; }
        }
        public double Effect
        {
            get => EffectValue;
            set { EffectValue = value; }
        }

        public void ApplyEffect(GameState gameState)
        {
            switch (Type)
            {
                case UpgradeType.ClickValue:
                    gameState.ClickValue *= EffectValue;
                    break;
                case UpgradeType.ProducerEfficiency:
                    var producer = gameState.Producers.FirstOrDefault(p => p.Name == TargetProducerName);
                    if (producer != null)
                    {
                        producer.BaseProduction *= EffectValue;
                    }
                    break;
                case UpgradeType.GlobalEfficiency:
                    gameState.GlobalEfficiencyMultiplier *= EffectValue;
                    break;
                case UpgradeType.MagicEssenceGain:
                    // TODO: Implement magic essence gain multiplier
                    break;
                case UpgradeType.CostReduction:
                    gameState.CostReductionMultiplier *= EffectValue;
                    break;
            }
        }

        public void Purchase()
        {
            if (!IsPurchased)
            {
                IsPurchased = true;
                Debug.WriteLine($"Upgrade purchased: {Name}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum UpgradeType
    {
        ClickValue,          // Increases the value of manual trades
        ProducerEfficiency,  // Increases production of a specific producer
        GlobalEfficiency,    // Increases production of all producers
        MagicEssenceGain,    // Increases magic essence generation
        CostReduction        // Reduces the cost of producers
    }
} 