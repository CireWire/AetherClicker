using System;
using System.Windows.Input;
using AetherClicker.Models;
using AetherClicker.Utils;
using AetherClicker.Commands;
using System.Diagnostics;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;

namespace AetherClicker.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private readonly GameState _gameState;
        private Producer? _selectedProducer;
        private Enhancement? _selectedEnhancement;
        private string _selectedBackground;
        private string _selectedSpecialization;

        public GameViewModel()
        {
            _gameState = new GameState();
            _selectedBackground = "Default";
            _selectedSpecialization = "None";
            InitializeCommands();
            InitializeEnhancements();
            Debug.WriteLine("GameViewModel initialized");
            // Add test upgrade for UI binding test
            _gameState.Upgrades.Add(new Upgrade("Test Upgrade", "Test Description", 100, 2.0, UpgradeType.GlobalEfficiency));
        }

        public GameViewModel(GameState gameState)
        {
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
            _selectedBackground = "Default";
            _selectedSpecialization = "None";
            _gameState.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(GameState.Coins):
                        OnPropertyChanged(nameof(Coins));
                        break;
                    case nameof(GameState.MagicEssence):
                        OnPropertyChanged(nameof(MagicEssence));
                        break;
                    case nameof(GameState.ClickValue):
                        OnPropertyChanged(nameof(ClickValue));
                        break;
                }
            };
            InitializeCommands();
            InitializeEnhancements();
            Debug.WriteLine("GameViewModel initialized with custom GameState");
        }

        private void InitializeCommands()
        {
            MakeTradeCommand = new RelayCommand(_ => 
            {
                _gameState.MakeTrade();
                OnPropertyChanged(nameof(Coins));
                OnPropertyChanged(nameof(MagicEssence));
            });

            PurchaseProducerCommand = new RelayCommand(producer => 
            {
                if (producer is Producer p)
                {
                    if (_gameState.TryPurchaseProducer(p))
                    {
                        OnPropertyChanged(nameof(Coins));
                        OnPropertyChanged(nameof(Producers));
                        ((RelayCommand)PurchaseProducerCommand).RaiseCanExecuteChanged();
                    }
                }
            }, producer => producer is Producer p && _gameState.Coins >= p.CurrentCost);

            PurchaseUpgradeCommand = new RelayCommand(upgrade => 
            {
                if (upgrade is Upgrade u)
                {
                    if (_gameState.TryPurchaseUpgrade(u))
                    {
                        OnPropertyChanged(nameof(Coins));
                        OnPropertyChanged(nameof(ClickValue));
                        OnPropertyChanged(nameof(Upgrades));
                        ((RelayCommand)PurchaseUpgradeCommand).RaiseCanExecuteChanged();
                    }
                }
            }, upgrade => upgrade is Upgrade u && _gameState.Coins >= u.Cost);

            PrestigeCommand = new RelayCommand(_ => 
            {
                if (_gameState.TryPrestige())
                {
                    OnPropertyChanged(nameof(Coins));
                    OnPropertyChanged(nameof(MagicEssence));
                    OnPropertyChanged(nameof(ClickValue));
                    OnPropertyChanged(nameof(Producers));
                    OnPropertyChanged(nameof(Upgrades));
                    OnPropertyChanged(nameof(Achievements));
                    ((RelayCommand)PrestigeCommand).RaiseCanExecuteChanged();
                }
            }, _ => _gameState.CanPrestige);

            SaveGameCommand = new RelayCommand(async _ => await SaveManager.SaveGameAsync(_gameState));
            
            LoadGameCommand = new RelayCommand(async _ => 
            {
                var saveData = await SaveManager.LoadGameAsync();
                if (saveData != null)
                {
                    _gameState.LoadFromSaveData(saveData);
                    OnPropertyChanged(nameof(Coins));
                    OnPropertyChanged(nameof(MagicEssence));
                    OnPropertyChanged(nameof(ClickValue));
                    OnPropertyChanged(nameof(Producers));
                    OnPropertyChanged(nameof(Upgrades));
                    OnPropertyChanged(nameof(Achievements));
                    ((RelayCommand)PurchaseProducerCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)PurchaseUpgradeCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)PrestigeCommand).RaiseCanExecuteChanged();
                }
            });

            PurchaseEnhancementCommand = new RelayCommand(_ =>
            {
                if (SelectedProducer != null && SelectedEnhancement != null)
                {
                    if (_gameState.TryPurchaseEnhancement(SelectedProducer, SelectedEnhancement))
                    {
                        OnPropertyChanged(nameof(MagicEssence));
                        OnPropertyChanged(nameof(Coins));
                        OnPropertyChanged(nameof(AvailableEnhancements));
                        UpdateAvailableEnhancements();
                        SelectedEnhancement = null;
                        ((RelayCommand)PurchaseEnhancementCommand).RaiseCanExecuteChanged();
                    }
                }
            }, _ => CanPurchaseEnhancement);
        }

        public double Coins
        {
            get => _gameState.Coins;
            set
            {
                if (_gameState.Coins != value)
                {
                    _gameState.Coins = value;
                    OnPropertyChanged();
                }
            }
        }

        public double MagicEssence
        {
            get => _gameState.MagicEssence;
            set
            {
                if (_gameState.MagicEssence != value)
                {
                    _gameState.MagicEssence = value;
                    OnPropertyChanged();
                }
            }
        }

        public double ClickValue
        {
            get => _gameState.ClickValue;
            set
            {
                if (_gameState.ClickValue != value)
                {
                    _gameState.ClickValue = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PlayerName => _gameState.PlayerName;
        public string CompanyName => _gameState.CompanyName;
        public Prestige Prestige => _gameState.Prestige;
        public ObservableCollection<Producer> Producers => _gameState.Producers;
        public ObservableCollection<Upgrade> Upgrades => _gameState.Upgrades;
        public ObservableCollection<Achievement> Achievements => _gameState.Achievements;
        public ObservableCollection<Enhancement> AvailableEnhancements { get; } = new ObservableCollection<Enhancement>();

        public Producer? SelectedProducer
        {
            get => _selectedProducer;
            set
            {
                if (_selectedProducer != value)
                {
                    _selectedProducer = value;
                    OnPropertyChanged();
                    SelectedEnhancement = null;
                    UpdateAvailableEnhancements();
                }
            }
        }

        public Enhancement? SelectedEnhancement
        {
            get => _selectedEnhancement;
            set
            {
                if (_selectedEnhancement != value)
                {
                    _selectedEnhancement = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanPurchaseEnhancement));
                }
            }
        }

        public bool CanPurchaseEnhancement => 
            SelectedProducer != null && 
            SelectedEnhancement != null && 
            MagicEssence >= SelectedEnhancement.Cost;

        private void UpdateAvailableEnhancements()
        {
            if (SelectedProducer == null)
            {
                AvailableEnhancements.Clear();
                OnPropertyChanged(nameof(AvailableEnhancements));
                return;
            }

            var availableEnhancements = AvailableEnhancements
                .Where(e => !SelectedProducer.Enhancements.Contains(e))
                .ToList();

            AvailableEnhancements.Clear();
            foreach (var enhancement in availableEnhancements)
            {
                AvailableEnhancements.Add(enhancement);
            }
            OnPropertyChanged(nameof(AvailableEnhancements));
        }

        public ICommand MakeTradeCommand { get; private set; } = null!;
        public ICommand PurchaseProducerCommand { get; private set; } = null!;
        public ICommand PurchaseUpgradeCommand { get; private set; } = null!;
        public ICommand PrestigeCommand { get; private set; } = null!;
        public ICommand SaveGameCommand { get; private set; } = null!;
        public ICommand LoadGameCommand { get; private set; } = null!;
        public ICommand PurchaseEnhancementCommand { get; private set; } = null!;

        private void InitializeEnhancements()
        {
            AvailableEnhancements.Clear();
            
            // Efficiency enhancements
            AvailableEnhancements.Add(new Enhancement(
                "Production Boost",
                "Increases the producer's base production by 25%",
                100,
                1.25,
                EnhancementType.Efficiency
            ));

            AvailableEnhancements.Add(new Enhancement(
                "Efficiency Mastery",
                "Increases the producer's base production by 50%",
                500,
                1.5,
                EnhancementType.Efficiency
            ));

            // Cost reduction enhancements
            AvailableEnhancements.Add(new Enhancement(
                "Cost Reduction",
                "Reduces the producer's base cost by 15%",
                200,
                0.85,
                EnhancementType.CostReduction
            ));

            AvailableEnhancements.Add(new Enhancement(
                "Bulk Discount",
                "Reduces the producer's base cost by 30%",
                1000,
                0.7,
                EnhancementType.CostReduction
            ));

            // Quantity bonus enhancements
            AvailableEnhancements.Add(new Enhancement(
                "Quantity Boost",
                "Increases the producer's quantity multiplier by 20%",
                300,
                1.2,
                EnhancementType.QuantityBonus
            ));

            AvailableEnhancements.Add(new Enhancement(
                "Mass Production",
                "Increases the producer's quantity multiplier by 40%",
                1500,
                1.4,
                EnhancementType.QuantityBonus
            ));
        }

        public string SelectedBackground
        {
            get => _selectedBackground;
            set
            {
                if (_selectedBackground != value)
                {
                    _selectedBackground = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SelectedSpecialization
        {
            get => _selectedSpecialization;
            set
            {
                if (_selectedSpecialization != value)
                {
                    _selectedSpecialization = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Enhancement> Enhancements => _gameState.Enhancements;

        public void AddProducer(Producer producer)
        {
            _gameState.Producers.Add(producer);
            OnPropertyChanged(nameof(Producers));
            ((RelayCommand)PurchaseProducerCommand).RaiseCanExecuteChanged();
        }

        public void AddUpgrade(Upgrade upgrade)
        {
            _gameState.Upgrades.Add(upgrade);
            OnPropertyChanged(nameof(Upgrades));
            ((RelayCommand)PurchaseUpgradeCommand).RaiseCanExecuteChanged();
        }

        public void AddEnhancement(Enhancement enhancement)
        {
            AvailableEnhancements.Add(enhancement);
            OnPropertyChanged(nameof(AvailableEnhancements));
            ((RelayCommand)PurchaseEnhancementCommand).RaiseCanExecuteChanged();
        }

        public void AddAchievement(Achievement achievement)
        {
            _gameState.Achievements.Add(achievement);
            OnPropertyChanged(nameof(Achievements));
        }

        public void AddCoins(double amount)
        {
            _gameState.Coins += amount;
            OnPropertyChanged(nameof(Coins));
            ((RelayCommand)PurchaseProducerCommand).RaiseCanExecuteChanged();
            ((RelayCommand)PurchaseUpgradeCommand).RaiseCanExecuteChanged();
        }

        public void AddMagicEssence(double amount)
        {
            _gameState.MagicEssence += amount;
            OnPropertyChanged(nameof(MagicEssence));
            ((RelayCommand)PurchaseEnhancementCommand).RaiseCanExecuteChanged();
        }

        public bool PurchaseProducer(Producer producer)
        {
            if (_gameState.PurchaseProducer(producer))
            {
                OnPropertyChanged(nameof(Coins));
                OnPropertyChanged(nameof(Producers));
                return true;
            }
            return false;
        }

        public bool PurchaseUpgrade(Upgrade upgrade)
        {
            if (_gameState.PurchaseUpgrade(upgrade))
            {
                OnPropertyChanged(nameof(Coins));
                OnPropertyChanged(nameof(Upgrades));
                return true;
            }
            return false;
        }

        public bool PurchaseEnhancement(Enhancement enhancement)
        {
            if (_gameState.PurchaseEnhancement(enhancement))
            {
                OnPropertyChanged(nameof(MagicEssence));
                OnPropertyChanged(nameof(Enhancements));
                return true;
            }
            return false;
        }

        public override event PropertyChangedEventHandler? PropertyChanged;

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 