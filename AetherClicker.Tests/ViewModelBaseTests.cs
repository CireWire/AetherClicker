using AetherClicker.ViewModels;
using System.ComponentModel;
using Xunit;

namespace AetherClicker.Tests;

public class ViewModelBaseTests
{
    private class TestViewModel : ViewModelBase
    {
        private string _testProperty = string.Empty;

        public string TestProperty
        {
            get => _testProperty;
            set => SetProperty(ref _testProperty, value);
        }
    }

    [Fact]
    public void SetProperty_WhenValueChanges_RaisesPropertyChanged()
    {
        // Arrange
        var viewModel = new TestViewModel();
        var propertyChangedRaised = false;
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(TestViewModel.TestProperty))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        viewModel.TestProperty = "New Value";

        // Assert
        Assert.True(propertyChangedRaised);
        Assert.Equal("New Value", viewModel.TestProperty);
    }

    [Fact]
    public void SetProperty_WhenValueDoesNotChange_DoesNotRaisePropertyChanged()
    {
        // Arrange
        var viewModel = new TestViewModel();
        viewModel.TestProperty = "Test Value";
        var propertyChangedRaised = false;
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(TestViewModel.TestProperty))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        viewModel.TestProperty = "Test Value";

        // Assert
        Assert.False(propertyChangedRaised);
    }

    [Fact]
    public void ViewModel_ImplementsINotifyPropertyChanged()
    {
        // Arrange & Act
        var viewModel = new TestViewModel();

        // Assert
        Assert.IsAssignableFrom<INotifyPropertyChanged>(viewModel);
    }
} 