using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AetherClicker.Models
{
    public class Producer : INotifyPropertyChanged
    {
        private string _name;
        private string _description;
        private double _baseCost;
        private double _baseProduction;
        private int _quantity;
        internal double _efficiencyMultiplier = 1.0;
        internal double _costReductionMultiplier = 1.0;
        internal double _quantityMultiplier = 1.0;
        private readonly List<Enhancement> _enhancements = new();

        public double EfficiencyMultiplierInternal => _efficiencyMultiplier;
        public double CostReductionMultiplierInternal => _costReductionMultiplier;
        public double QuantityMultiplierInternal => _quantityMultiplier;

        public Producer(string name, string description, double baseCost, double baseProduction)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _description = description ?? throw new ArgumentNullException(nameof(description));
            _baseCost = baseCost;
            _baseProduction = baseProduction;
            Debug.WriteLine($"Producer created: {name}, Base production: {baseProduction}/sec");
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
                    OnPropertyChanged(nameof(CurrentCost));
                }
            }
        }

        public double BaseProduction
        {
            get => _baseProduction;
            set
            {
                if (_baseProduction != value)
                {
                    _baseProduction = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentProduction));
                }
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentCost));
                    OnPropertyChanged(nameof(CurrentProduction));
                }
            }
        }

        public double EfficiencyMultiplier
        {
            get => _efficiencyMultiplier;
            set
            {
                if (_efficiencyMultiplier != value)
                {
                    _efficiencyMultiplier = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentProduction));
                }
            }
        }

        public double CostReductionMultiplier
        {
            get => _costReductionMultiplier;
            set
            {
                if (_costReductionMultiplier != value)
                {
                    _costReductionMultiplier = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentCost));
                }
            }
        }

        public double QuantityMultiplier
        {
            get => _quantityMultiplier;
            set
            {
                if (_quantityMultiplier != value)
                {
                    _quantityMultiplier = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentProduction));
                }
            }
        }

        public double CurrentCost => CalculateCost();
        public double CurrentProduction => CalculateProduction();

        public List<Enhancement> Enhancements => _enhancements;

        public void Purchase()
        {
            Quantity++;
            Debug.WriteLine($"Producer purchased: {Name}, New quantity: {Quantity}");
        }

        public void ApplyEnhancement(Enhancement enhancement)
        {
            if (enhancement == null)
            {
                throw new ArgumentNullException(nameof(enhancement));
            }

            if (!_enhancements.Contains(enhancement))
            {
                _enhancements.Add(enhancement);
                Debug.WriteLine($"Enhancement applied to producer: {enhancement.Name} -> {Name}");
                OnPropertyChanged(nameof(Enhancements));
                OnPropertyChanged(nameof(CurrentProduction));
            }
        }

        public double CalculateCost()
        {
            return _baseCost * Math.Pow(1.15, _quantity) * _costReductionMultiplier;
        }

        public double CalculateProduction()
        {
            var baseOutput = _baseProduction * _quantity * _efficiencyMultiplier * _quantityMultiplier;
            var enhancementMultiplier = _enhancements.Where(e => e.IsActive).Sum(e => e.Effect);
            return baseOutput * (1 + enhancementMultiplier);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 