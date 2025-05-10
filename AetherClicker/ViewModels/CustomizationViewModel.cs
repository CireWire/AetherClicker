using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using AetherClicker.Models;
using AetherClicker.Views;
using AetherClicker.Commands;

namespace AetherClicker.ViewModels
{
    public class CustomizationViewModel : INotifyPropertyChanged
    {
        private readonly ICustomizationWindow _window;
        private string _playerName = string.Empty;
        private CustomizationOption? _selectedBackground;
        private CustomizationOption? _selectedSpecialization;
        private string _companyName = string.Empty;
        private CustomizationOption? _selectedCompanyType;
        private CustomizationOption? _selectedLocation;
        private CustomizationOption? _selectedStartingBonus;
        private bool _isTransitioningToGame;

        private readonly ObservableCollection<CustomizationOption> _backgrounds;
        private readonly ObservableCollection<CustomizationOption> _specializations;
        private readonly ObservableCollection<CustomizationOption> _companyTypes;
        private readonly ObservableCollection<CustomizationOption> _locations;
        private readonly ObservableCollection<CustomizationOption> _startingBonuses;

        private readonly GameViewModel _gameViewModel;
        private ObservableCollection<string> _availableBackgrounds;
        private ObservableCollection<string> _availableSpecializations;

        public CustomizationViewModel(ICustomizationWindow window, GameViewModel gameViewModel)
        {
            _window = window;
            _gameViewModel = gameViewModel ?? throw new ArgumentNullException(nameof(gameViewModel));
            _backgrounds = new ObservableCollection<CustomizationOption>();
            _specializations = new ObservableCollection<CustomizationOption>();
            _companyTypes = new ObservableCollection<CustomizationOption>();
            _locations = new ObservableCollection<CustomizationOption>();
            _startingBonuses = new ObservableCollection<CustomizationOption>();

            _availableBackgrounds = new ObservableCollection<string> { "Default", "Dark", "Light" };
            _availableSpecializations = new ObservableCollection<string> { "None", "Producer", "Upgrade", "Enhancement" };

            StartGameCommand = new RelayCommand(
                execute: (object? _) => ExecuteStartGame(),
                canExecute: (object? _) => CanStartGame()
            );
            InitializeOptions();
        }

        private void InitializeOptions()
        {
            _backgrounds.Clear();
            _backgrounds.Add(new CustomizationOption("Noble", "Born into wealth and privilege, you start with more coins but less magical knowledge."));
            _backgrounds.Add(new CustomizationOption("Merchant", "A natural trader, you have a better understanding of market efficiency."));
            _backgrounds.Add(new CustomizationOption("Scholar", "Your magical studies have given you a head start in arcane knowledge."));

            _specializations.Clear();
            _specializations.Add(new CustomizationOption("Alchemy", "Expert in potion brewing and magical compounds."));
            _specializations.Add(new CustomizationOption("Enchanting", "Master of imbuing items with magical properties."));
            _specializations.Add(new CustomizationOption("Divination", "Skilled in predicting market trends and magical fluctuations."));

            _companyTypes.Clear();
            _companyTypes.Add(new CustomizationOption("Trading Guild", "Focus on traditional magical goods and services."));
            _companyTypes.Add(new CustomizationOption("Research Consortium", "Specialize in magical research and development."));
            _companyTypes.Add(new CustomizationOption("Artifact Dealers", "Deal in rare and powerful magical artifacts."));

            _locations.Clear();
            _locations.Add(new CustomizationOption("Arcane District", "The heart of magical commerce, with high competition but better prices."));
            _locations.Add(new CustomizationOption("Scholar's Quarter", "Near major magical institutions, offering unique research opportunities."));
            _locations.Add(new CustomizationOption("Merchant's Row", "A bustling trade hub with diverse magical goods."));

            _startingBonuses.Clear();
            _startingBonuses.Add(new CustomizationOption("Efficiency", "15% boost to production efficiency."));
            _startingBonuses.Add(new CustomizationOption("Cost Reduction", "5% reduction in all costs."));
            _startingBonuses.Add(new CustomizationOption("Starting Capital", "50% more starting coins."));
        }

        public string PlayerName
        {
            get => _playerName;
            set
            {
                if (_playerName != value)
                {
                    _playerName = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public CustomizationOption? SelectedBackground
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

        public CustomizationOption? SelectedSpecialization
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

        public string CompanyName
        {
            get => _companyName;
            set
            {
                if (_companyName != value)
                {
                    _companyName = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public CustomizationOption? SelectedCompanyType
        {
            get => _selectedCompanyType;
            set
            {
                if (_selectedCompanyType != value)
                {
                    _selectedCompanyType = value;
                    OnPropertyChanged();
                }
            }
        }

        public CustomizationOption? SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                if (_selectedLocation != value)
                {
                    _selectedLocation = value;
                    OnPropertyChanged();
                }
            }
        }

        public CustomizationOption? SelectedStartingBonus
        {
            get => _selectedStartingBonus;
            set
            {
                if (_selectedStartingBonus != value)
                {
                    _selectedStartingBonus = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsTransitioningToGame
        {
            get => _isTransitioningToGame;
            private set
            {
                if (_isTransitioningToGame != value)
                {
                    _isTransitioningToGame = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<CustomizationOption> Backgrounds => _backgrounds;
        public ObservableCollection<CustomizationOption> Specializations => _specializations;
        public ObservableCollection<CustomizationOption> CompanyTypes => _companyTypes;
        public ObservableCollection<CustomizationOption> Locations => _locations;
        public ObservableCollection<CustomizationOption> StartingBonuses => _startingBonuses;

        public ObservableCollection<string> AvailableBackgrounds
        {
            get => _availableBackgrounds;
            set
            {
                if (_availableBackgrounds != value)
                {
                    _availableBackgrounds = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> AvailableSpecializations
        {
            get => _availableSpecializations;
            set
            {
                if (_availableSpecializations != value)
                {
                    _availableSpecializations = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand StartGameCommand { get; }

        private bool CanStartGame()
        {
            return !string.IsNullOrWhiteSpace(PlayerName) && !string.IsNullOrWhiteSpace(CompanyName);
        }

        private void ExecuteStartGame()
        {
            try
            {
                if (IsTransitioningToGame)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(PlayerName) || string.IsNullOrWhiteSpace(CompanyName))
                {
                    MessageBox.Show("Please enter both a player name and company name.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (SelectedBackground == null || SelectedSpecialization == null || 
                    SelectedCompanyType == null || SelectedLocation == null || 
                    SelectedStartingBonus == null)
                {
                    MessageBox.Show("Please select all customization options.", "Missing Selections", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                IsTransitioningToGame = true;

                // Create and initialize GameState with customization data
                var gameState = new GameState
                {
                    PlayerName = PlayerName,
                    CompanyName = CompanyName
                };

                // Apply starting bonuses based on selections
                if (SelectedBackground.Name == "Noble")
                {
                    gameState.Coins = 1000; // More starting coins
                }
                else if (SelectedBackground.Name == "Scholar")
                {
                    gameState.MagicEssence = 50; // More starting magic essence
                }

                if (SelectedStartingBonus.Name == "Efficiency")
                {
                    gameState.GlobalEfficiencyMultiplier = 1.15;
                }
                else if (SelectedStartingBonus.Name == "Cost Reduction")
                {
                    gameState.CostReductionMultiplier = 0.95;
                }
                else if (SelectedStartingBonus.Name == "Starting Capital")
                {
                    gameState.Coins *= 1.5;
                }

                // Create and initialize GameViewModel
                var gameViewModel = new GameViewModel(gameState);
                
                // Create and show the main window
                var mainWindow = new MainWindow
                {
                    DataContext = gameViewModel
                };
                mainWindow.Show();

                // Close this window
                _window.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while starting the game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                IsTransitioningToGame = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CustomizationOption
    {
        public string Name { get; }
        public string Description { get; }

        public CustomizationOption(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
} 