using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AetherClicker.Models
{
    public class Enhancement : INotifyPropertyChanged
    {
        private string _name;
        private string _description;
        private double _baseCost;
        private double _effectValue;
        private bool _isPurchased;
        private bool _isActive;
        private EnhancementType _type;

        public Enhancement(string name, string description, double baseCost, double effectValue, EnhancementType type)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _description = description ?? throw new ArgumentNullException(nameof(description));
            _baseCost = baseCost;
            _effectValue = effectValue;
            _type = type;
            Debug.WriteLine($"Enhancement created: {name}, Effect: {effectValue}");
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
                    OnPropertyChanged(nameof(Cost));
                }
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

        public double EffectValue
        {
            get => _effectValue;
            set
            {
                if (_effectValue != value)
                {
                    _effectValue = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Effect));
                }
            }
        }

        public EnhancementType Type
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

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged();
                }
            }
        }

        public void Purchase()
        {
            if (!IsPurchased)
            {
                IsPurchased = true;
                Debug.WriteLine($"Enhancement purchased: {Name}");
            }
        }

        public void ApplyEffect(Producer producer)
        {
            if (IsPurchased && !IsActive)
            {
                producer.ApplyEnhancement(this);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum EnhancementType
    {
        Efficiency,    // Increases producer's base production
        CostReduction, // Reduces producer's base cost
        QuantityBonus  // Adds a multiplier to producer's quantity
    }
} 