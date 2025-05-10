# AetherClicker - Developer Guide

## Table of Contents
1. [Project Structure](#project-structure)
2. [Architecture](#architecture)
3. [Core Systems](#core-systems)
4. [UI Framework](#ui-framework)
5. [Testing](#testing)
6. [Contributing](#contributing)

## Project Structure

### Solution Organization
```
ArcanaTradingCompany/
├── ArcanaTradingCompany/           # Main game project
│   ├── Models/                     # Game state and business logic
│   ├── ViewModels/                 # MVVM view models
│   ├── Views/                      # WPF user interfaces
│   ├── Services/                   # Game services
│   └── Utils/                      # Utility classes
├── ArcanaTradingCompany.Tests/     # Test project
│   ├── UnitTests/                  # Unit tests
│   ├── IntegrationTests/           # Integration tests
│   └── UITests/                    # UI tests
└── docs/                           # Documentation
```

### Key Files
- `GameState.cs`: Core game state management
- `GameViewModel.cs`: Main view model
- `MainWindow.xaml`: Primary game window
- `SaveLoadService.cs`: Save/load functionality
- `PrestigeService.cs`: Prestige system
- `Producer.cs`: Producer implementation
- `Upgrade.cs`: Upgrade system
- `Achievement.cs`: Achievement system

## Architecture

### Design Patterns
1. **MVVM (Model-View-ViewModel)**
   - Models: Game state and business logic
   - Views: WPF XAML interfaces
   - ViewModels: Data binding and commands

2. **Dependency Injection**
   - Service registration in `App.xaml.cs`
   - Interface-based design
   - Loose coupling

3. **Command Pattern**
   - `ICommand` implementation
   - Command binding in XAML
   - Async command support

### Data Flow
1. **User Input**
   ```
   View -> ViewModel -> Model -> Services
   ```

2. **State Updates**
   ```
   Model -> ViewModel -> View
   ```

3. **Save/Load**
   ```
   Model -> SaveLoadService -> File System
   ```

## Core Systems

### Game State
```csharp
public class GameState
{
    public string PlayerName { get; set; }
    public string CompanyName { get; set; }
    public double Coins { get; set; }
    public double MagicEssence { get; set; }
    public int PrestigeLevel { get; set; }
    public ObservableCollection<Producer> Producers { get; set; }
    public ObservableCollection<Upgrade> Upgrades { get; set; }
    public ObservableCollection<Achievement> Achievements { get; set; }
}
```

### Producer System
```csharp
public class Producer
{
    public string Name { get; set; }
    public double BaseCost { get; set; }
    public double BaseProduction { get; set; }
    public int Quantity { get; set; }
    public double ProductionMultiplier { get; set; }
    public double CostMultiplier { get; set; }
}
```

### Upgrade System
```csharp
public class Upgrade
{
    public string Name { get; set; }
    public double Cost { get; set; }
    public double Effect { get; set; }
    public bool IsPurchased { get; set; }
    public UpgradeType Type { get; set; }
}
```

### Achievement System
```csharp
public class Achievement
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsUnlocked { get; set; }
    public AchievementType Type { get; set; }
    public double Requirement { get; set; }
}
```

### Prestige System
```csharp
public class PrestigeService
{
    public double CalculatePrestigePoints(GameState state)
    public void PerformPrestige(GameState state)
    public bool CanPrestige(GameState state)
}
```

## UI Framework

### XAML Structure
```xaml
<Window>
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>
        
        <!-- Top Bar -->
        <StackPanel Grid.Row="0">
            <TextBlock Text="{Binding Coins}"/>
            <TextBlock Text="{Binding MagicEssence}"/>
        </StackPanel>
        
        <!-- Main Content -->
        <TabControl Grid.Row="1">
            <TabItem Header="Producers">
                <ItemsControl ItemsSource="{Binding Producers}"/>
            </TabItem>
            <TabItem Header="Upgrades">
                <ItemsControl ItemsSource="{Binding Upgrades}"/>
            </TabItem>
        </TabControl>
    </Grid>
</Window>
```

### Data Binding
1. **Property Change Notification**
   ```csharp
   public class ViewModelBase : INotifyPropertyChanged
   {
       public event PropertyChangedEventHandler PropertyChanged;
       protected void OnPropertyChanged(string propertyName)
       {
           PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
       }
   }
   ```

2. **Command Binding**
   ```csharp
   public ICommand MakeTradeCommand { get; }
   public ICommand PurchaseCommand { get; }
   public ICommand PrestigeCommand { get; }
   ```

### Styles and Templates
1. **Resource Dictionary**
   ```xaml
   <ResourceDictionary>
       <Style TargetType="Button" x:Key="GameButton">
           <Setter Property="Background" Value="#FF4A4A4A"/>
           <Setter Property="Foreground" Value="White"/>
       </Style>
   </ResourceDictionary>
   ```

2. **Data Templates**
   ```xaml
   <DataTemplate DataType="{x:Type models:Producer}">
       <StackPanel>
           <TextBlock Text="{Binding Name}"/>
           <TextBlock Text="{Binding Production}"/>
       </StackPanel>
   </DataTemplate>
   ```

## Testing

### Unit Tests
```csharp
[Fact]
public void Producer_CalculateProduction_ReturnsCorrectValue()
{
    var producer = new Producer
    {
        BaseProduction = 10,
        Quantity = 2,
        ProductionMultiplier = 1.5
    };
    
    var production = producer.CalculateProduction();
    Assert.Equal(30, production);
}
```

### Integration Tests
```csharp
[Fact]
public async Task SaveLoad_GameState_PreservesAllProperties()
{
    var gameState = CreateTestGameState();
    await _saveLoadService.SaveGameAsync(gameState, "test.sav");
    var loadedState = await _saveLoadService.LoadGameAsync("test.sav");
    
    Assert.Equal(gameState.Coins, loadedState.Coins);
    Assert.Equal(gameState.MagicEssence, loadedState.MagicEssence);
}
```

### UI Tests
```csharp
[Fact]
public async Task MainWindow_ButtonClick_UpdatesCoins()
{
    var window = new MainWindow();
    var button = window.FindName("MakeTradeButton") as Button;
    
    button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
    await Task.Delay(100);
    
    var coinsText = window.FindName("CoinsText") as TextBlock;
    Assert.Equal("1", coinsText.Text);
}
```

## Contributing

### Development Setup
1. **Prerequisites**
   - Visual Studio 2022
   - .NET 6.0 SDK
   - Git

2. **Getting Started**
   ```bash
   git clone https://github.com/yourusername/ArcanaTradingCompany.git
   cd ArcanaTradingCompany
   dotnet restore
   dotnet build
   ```

3. **Running Tests**
   ```bash
   dotnet test
   ```

### Coding Standards
1. **Naming Conventions**
   - PascalCase for public members
   - camelCase for private members
   - _prefix for private fields

2. **Documentation**
   - XML comments for public APIs
   - README updates for new features
   - Code comments for complex logic

3. **Testing Requirements**
   - Unit tests for new features
   - Integration tests for system interactions
   - UI tests for new interfaces

### Pull Request Process
1. Create feature branch
2. Implement changes
3. Add tests
4. Update documentation
5. Submit pull request
6. Address review comments
7. Merge after approval

### Release Process
1. Update version numbers
2. Create release notes
3. Build release package
4. Run full test suite
5. Create GitHub release
6. Deploy to distribution 