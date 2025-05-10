using AetherClicker.Models;
using AetherClicker.ViewModels;
using System.ComponentModel;
using System.Windows.Input;
using Xunit;

namespace AetherClicker.Tests;

public class UIBindingTests : IDisposable
{
    private readonly GameViewModel _viewModel;
    private readonly List<IDisposable> _disposables = new();

    public UIBindingTests()
    {
        _viewModel = CreateTestGameViewModel();
    }

    public void Dispose()
    {
        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }
    }

    private GameViewModel CreateTestGameViewModel()
    {
        var gameState = new GameState
        {
            PlayerName = "TestPlayer",
            CompanyName = "TestCompany",
            SelectedBackground = "Mystic",
            SelectedSpecialization = "Alchemy"
        };
        return new GameViewModel(gameState);
    }

    private void SetupPropertyChangedHandler(Action<string> onPropertyChanged)
    {
        var handler = new PropertyChangedEventHandler((s, e) => 
        {
            if (e.PropertyName != null)
            {
                onPropertyChanged(e.PropertyName);
            }
        });
        _viewModel.PropertyChanged += handler;
        _disposables.Add(new DisposableAction(() => _viewModel.PropertyChanged -= handler));
    }

    [Theory]
    [InlineData(nameof(GameViewModel.Coins), 100.0)]
    [InlineData(nameof(GameViewModel.MagicEssence), 50.0)]
    public void ResourceProperties_NotifyPropertyChanged(string propertyName, double amount)
    {
        // Arrange
        var propertyChangedRaised = false;
        SetupPropertyChangedHandler(p => 
        {
            if (p == propertyName)
            {
                propertyChangedRaised = true;
            }
        });

        // Act
        if (propertyName == nameof(GameViewModel.Coins))
        {
            _viewModel.AddCoins(amount);
        }
        else
        {
            _viewModel.AddMagicEssence(amount);
        }

        // Assert
        Assert.True(propertyChangedRaised);
        Assert.Equal(amount, propertyName == nameof(GameViewModel.Coins) ? _viewModel.Coins : _viewModel.MagicEssence);
    }

    [Theory]
    [InlineData(nameof(GameViewModel.Producers))]
    [InlineData(nameof(GameViewModel.Upgrades))]
    [InlineData(nameof(GameViewModel.Achievements))]
    [InlineData(nameof(GameViewModel.AvailableEnhancements))]
    public void CollectionProperties_NotifyPropertyChanged(string propertyName)
    {
        // Arrange
        var propertyChangedRaised = false;
        SetupPropertyChangedHandler(p => 
        {
            if (p == propertyName)
            {
                propertyChangedRaised = true;
            }
        });

        // Act
        switch (propertyName)
        {
            case nameof(GameViewModel.Producers):
                var producer = new Producer("Test Producer", "Test Description", 100, 10);
                _viewModel.AddProducer(producer);
                break;
            case nameof(GameViewModel.Upgrades):
                var upgrade = new Upgrade("Test Upgrade", "Test Description", 100, 1.5, UpgradeType.ClickValue, "producer");
                _viewModel.AddUpgrade(upgrade);
                break;
            case nameof(GameViewModel.Achievements):
                var achievement = new Achievement("Test Achievement", "Test Description", 100, AchievementType.CoinsEarned, 1.1, "producer");
                _viewModel.AddAchievement(achievement);
                break;
            case nameof(GameViewModel.AvailableEnhancements):
                var enhancement = new Enhancement("Test Enhancement", "Test Description", 100, 1.5, EnhancementType.Efficiency);
                _viewModel.AvailableEnhancements.Add(enhancement);
                break;
        }

        // Assert
        Assert.True(propertyChangedRaised);
    }

    [Fact]
    public void CommandBindings_UpdateCanExecute()
    {
        // Arrange
        var producer = new Producer("Test Producer", "Test Description", 100, 10);
        _viewModel.AddProducer(producer);

        // Act & Assert
        Assert.False(((ICommand)_viewModel.PurchaseProducerCommand).CanExecute(producer));
        _viewModel.AddCoins(200);
        Assert.True(((ICommand)_viewModel.PurchaseProducerCommand).CanExecute(producer));
    }

    [Fact]
    public void CommandBindings_ExecuteCommands()
    {
        // Arrange
        var producer = new Producer("Test Producer", "Test Description", 100, 10);
        _viewModel.AddProducer(producer);
        _viewModel.AddCoins(200);

        // Act
        ((ICommand)_viewModel.PurchaseProducerCommand).Execute(producer);

        // Assert
        Assert.Equal(1, producer.Quantity);
        Assert.Equal(100, _viewModel.Coins);
    }

    private class DisposableAction : IDisposable
    {
        private readonly Action _action;

        public DisposableAction(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _action();
        }
    }
} 