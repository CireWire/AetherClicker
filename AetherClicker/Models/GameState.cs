using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using System.Diagnostics;
using System.Linq;
using AetherClicker.Utils;

namespace AetherClicker.Models
{
    public class GameState : INotifyPropertyChanged
    {
        private double _coins;
        private double _magicEssence;
        private double _clickValue;
        private string _playerName = string.Empty;
        private string _companyName = string.Empty;
        private readonly DispatcherTimer _gameTimer;
        private readonly ObservableCollection<Producer> _producers;
        private readonly ObservableCollection<Upgrade> _upgrades;
        private readonly ObservableCollection<Achievement> _achievements;
        private readonly Prestige _prestige;
        private DateTime _lastTick;
        private double _globalEfficiencyMultiplier = 1.0;
        private double _costReductionMultiplier = 1.0;
        private double _totalCoinsEarned;
        private TimeSpan _timePlayed;
        private readonly ObservableCollection<Enhancement> _enhancements;

        public GameState()
        {
            _coins = 0;
            _magicEssence = 0;
            _clickValue = 1;
            _producers = new ObservableCollection<Producer>();
            _upgrades = new ObservableCollection<Upgrade>();
            _achievements = new ObservableCollection<Achievement>();
            _prestige = new Prestige();
            _enhancements = new ObservableCollection<Enhancement>();
            InitializeProducers();
            InitializeUpgrades();
            InitializeAchievements();

            // Ensure we're on the UI thread
            _gameTimer = new DispatcherTimer(DispatcherPriority.Normal, Dispatcher.CurrentDispatcher)
            {
                Interval = TimeSpan.FromMilliseconds(100) // Update 10 times per second
            };
            _gameTimer.Tick += GameTimer_Tick;
            _lastTick = DateTime.Now;
            _gameTimer.Start();
            
            Debug.WriteLine("GameState initialized");
        }

        private void InitializeProducers()
        {
            Producers.Add(new Producer("Apprentice", "A novice trader learning the arcane arts", 15, 0.1));
            Producers.Add(new Producer("Enchanter", "A skilled practitioner of magical commerce", 225, 1));
            Producers.Add(new Producer("Alchemy Table", "Transforms mundane materials into valuable goods", 3_375, 8));
            Producers.Add(new Producer("Magical Clerk", "An efficient assistant with arcane knowledge", 50_625, 47));
            Producers.Add(new Producer("Automated Stall", "A self-operating magical marketplace", 759_375, 260));
            Producers.Add(new Producer("Pocket Dimension", "A personal trading space outside normal reality", 11_390_625, 1_400));
            Producers.Add(new Producer("Market Familiar", "A magical creature that trades on your behalf", 170_859_375, 7_800));
            Producers.Add(new Producer("Arcane Portal", "A gateway to other trading dimensions", 2_562_890_625, 44_000));
            Producers.Add(new Producer("Trade Golem", "A massive construct built for commerce", 38_443_359_375, 260_000));
            Producers.Add(new Producer("Aethereal Exchange", "A nexus of interdimensional trade", 576_650_390_625, 1_600_000));
            
            Debug.WriteLine("Producers initialized");
        }

        private void InitializeUpgrades()
        {
            // Click value upgrades
            Upgrades.Add(new Upgrade("Better Deals", "Increase the value of your trades by 50%", 100, 1.5, UpgradeType.ClickValue));
            Upgrades.Add(new Upgrade("Master Negotiator", "Double the value of your trades", 500, 2.0, UpgradeType.ClickValue));
            Upgrades.Add(new Upgrade("Arcane Trading", "Triple the value of your trades", 2500, 3.0, UpgradeType.ClickValue));

            // Global efficiency upgrades
            Upgrades.Add(new Upgrade("Trading Network", "Increase all production by 25%", 1000, 1.25, UpgradeType.GlobalEfficiency));
            Upgrades.Add(new Upgrade("Magical Synergy", "Double all production", 5000, 2.0, UpgradeType.GlobalEfficiency));
            Upgrades.Add(new Upgrade("Dimensional Trading", "Triple all production", 25000, 3.0, UpgradeType.GlobalEfficiency));

            // Cost reduction upgrades
            Upgrades.Add(new Upgrade("Bulk Discount", "Reduce producer costs by 10%", 2000, 0.9, UpgradeType.CostReduction));
            Upgrades.Add(new Upgrade("Magical Contracts", "Reduce producer costs by 25%", 10000, 0.75, UpgradeType.CostReduction));
            Upgrades.Add(new Upgrade("Dimensional Bargaining", "Reduce producer costs by 50%", 50000, 0.5, UpgradeType.CostReduction));

            // Producer-specific upgrades
            foreach (var producer in Producers)
            {
                Upgrades.Add(new Upgrade(
                    $"{producer.Name} Mastery",
                    $"Double {producer.Name}'s production",
                    producer.BaseCost * 5,
                    2.0,
                    UpgradeType.ProducerEfficiency,
                    producer.Name
                ));
            }

            Debug.WriteLine("Upgrades initialized");
        }

        private void InitializeAchievements()
        {
            // Coin milestones
            Achievements.Add(new Achievement("First Trade", "Earn your first coin", 1, AchievementType.CoinsEarned, 1.1));
            Achievements.Add(new Achievement("Novice Trader", "Earn 100 coins", 100, AchievementType.CoinsEarned, 1.1));
            Achievements.Add(new Achievement("Skilled Trader", "Earn 1,000 coins", 1000, AchievementType.CoinsEarned, 1.2));
            Achievements.Add(new Achievement("Master Trader", "Earn 10,000 coins", 10000, AchievementType.CoinsEarned, 1.3));
            Achievements.Add(new Achievement("Legendary Trader", "Earn 100,000 coins", 100000, AchievementType.CoinsEarned, 1.5));

            // Producer milestones
            Achievements.Add(new Achievement("First Producer", "Purchase your first producer", 1, AchievementType.TotalProducers, 1.1));
            Achievements.Add(new Achievement("Producer Empire", "Own 10 producers", 10, AchievementType.TotalProducers, 1.2));
            Achievements.Add(new Achievement("Trading Dynasty", "Own 50 producers", 50, AchievementType.TotalProducers, 1.3));

            // Upgrade milestones
            Achievements.Add(new Achievement("First Upgrade", "Purchase your first upgrade", 1, AchievementType.TotalUpgrades, 1.1));
            Achievements.Add(new Achievement("Upgrade Master", "Purchase 5 upgrades", 5, AchievementType.TotalUpgrades, 1.2));
            Achievements.Add(new Achievement("Upgrade Legend", "Purchase 10 upgrades", 10, AchievementType.TotalUpgrades, 1.3));

            // Click value milestones
            Achievements.Add(new Achievement("Click Power", "Reach a click value of 10", 10, AchievementType.ClickValue, 1.1));
            Achievements.Add(new Achievement("Click Master", "Reach a click value of 100", 100, AchievementType.ClickValue, 1.2));
            Achievements.Add(new Achievement("Click Legend", "Reach a click value of 1,000", 1000, AchievementType.ClickValue, 1.3));

            // Time played milestones
            Achievements.Add(new Achievement("Dedicated Trader", "Play for 1 hour", 3600, AchievementType.TimePlayed, 1.1));
            Achievements.Add(new Achievement("Veteran Trader", "Play for 5 hours", 18000, AchievementType.TimePlayed, 1.2));
            Achievements.Add(new Achievement("Master Trader", "Play for 10 hours", 36000, AchievementType.TimePlayed, 1.3));

            Debug.WriteLine("Achievements initialized");
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                var now = DateTime.Now;
                var elapsed = now - _lastTick;
                _lastTick = now;

                // Update time played
                TimePlayed = TimePlayed.Add(elapsed);
                UpdateAchievementProgress(AchievementType.TimePlayed, TimePlayed.TotalSeconds);

                double production = 0;
                foreach (var producer in Producers)
                {
                    if (producer.Quantity > 0)
                    {
                        production += producer.CurrentProduction;
                    }
                }
                
                // Apply global efficiency multiplier and prestige multipliers
                production *= _globalEfficiencyMultiplier * _prestige.ProducerEfficiencyMultiplier;
                
                // Calculate production based on actual elapsed time
                var coinsToAdd = production * elapsed.TotalSeconds * _prestige.CoinMultiplier;
                if (coinsToAdd > 0)
                {
                    Coins += coinsToAdd;
                    TotalCoinsEarned += coinsToAdd;
                    UpdateAchievementProgress(AchievementType.CoinsEarned, TotalCoinsEarned);
                    Debug.WriteLine($"Timer tick - Production: {production}/sec, Added: {coinsToAdd}, Total coins: {Coins}");
                }

                // Generate Magic Essence based on total production (increased rate to 0.05)
                var essenceToAdd = production * elapsed.TotalSeconds * 0.05 * _prestige.MagicEssenceMultiplier;
                if (essenceToAdd > 0)
                {
                    MagicEssence += essenceToAdd;
                    UpdateAchievementProgress(AchievementType.MagicEssence, MagicEssence);
                    Debug.WriteLine($"Timer tick - Added {essenceToAdd} Magic Essence, Total: {MagicEssence}");
                }

                // Update achievements
                foreach (var achievement in Achievements.Where(a => !a.IsUnlocked))
                {
                    achievement.UpdateProgress(this);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GameTimer_Tick: {ex}");
            }
        }

        public double Coins
        {
            get => _coins;
            set
            {
                if (_coins != value)
                {
                    _coins = value;
                    Debug.WriteLine($"Coins property changed to: {_coins}");
                    OnPropertyChanged();
                }
            }
        }

        public double MagicEssence
        {
            get => _magicEssence;
            set
            {
                if (_magicEssence != value)
                {
                    _magicEssence = value;
                    OnPropertyChanged();
                    UpdateAchievementProgress(AchievementType.MagicEssence, value);
                }
            }
        }

        public double ClickValue
        {
            get => _clickValue;
            set
            {
                if (_clickValue != value)
                {
                    _clickValue = value;
                    OnPropertyChanged();
                    UpdateAchievementProgress(AchievementType.ClickValue, value);
                }
            }
        }

        public string PlayerName
        {
            get => _playerName;
            set
            {
                _playerName = value;
                OnPropertyChanged();
            }
        }

        public string CompanyName
        {
            get => _companyName;
            set
            {
                _companyName = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Producer> Producers => _producers;
        public ObservableCollection<Upgrade> Upgrades => _upgrades;
        public ObservableCollection<Achievement> Achievements => _achievements;
        public ObservableCollection<Enhancement> Enhancements => _enhancements;

        public void MakeTrade()
        {
            double tradeValue = ClickValue * _prestige.ClickValueMultiplier;
            Coins += tradeValue;
            TotalCoinsEarned += tradeValue;
            UpdateAchievementProgress(AchievementType.CoinsEarned, TotalCoinsEarned);
            Debug.WriteLine($"Made trade worth {tradeValue} coins");
        }

        public bool TryPurchaseProducer(Producer producer)
        {
            if (producer == null)
            {
                Debug.WriteLine("Cannot purchase producer: producer is null");
                return false;
            }

            if (Coins < producer.CurrentCost)
            {
                Debug.WriteLine($"Cannot purchase producer: not enough coins (have {Coins}, need {producer.CurrentCost})");
                return false;
            }

            Coins -= producer.CurrentCost;
            producer.Quantity++;
            UpdateAchievementProgress(AchievementType.TotalProducers, Producers.Sum(p => p.Quantity));
            if (!string.IsNullOrEmpty(producer.Name))
            {
                UpdateAchievementProgress(AchievementType.ProducerQuantity, producer.Quantity, producer.Name);
            }
            Debug.WriteLine($"Purchased producer {producer.Name}, new quantity: {producer.Quantity}");
            return true;
        }

        public bool TryPurchaseUpgrade(Upgrade upgrade)
        {
            if (upgrade == null)
            {
                Debug.WriteLine("Cannot purchase upgrade: upgrade is null");
                return false;
            }

            if (Coins < upgrade.CurrentCost)
            {
                Debug.WriteLine($"Cannot purchase upgrade: not enough coins (have {Coins}, need {upgrade.CurrentCost})");
                return false;
            }

            Coins -= upgrade.CurrentCost;
            upgrade.Level++;
            upgrade.ApplyEffect(this);
            UpdateAchievementProgress(AchievementType.TotalUpgrades, Upgrades.Count(u => u.IsPurchased));
            Debug.WriteLine($"Purchased upgrade {upgrade.Name}, new level: {upgrade.Level}");
            return true;
        }

        public bool TryPrestige()
        {
            if (Coins < _prestige.PrestigeCost)
            {
                Debug.WriteLine($"Cannot prestige: not enough coins (have {Coins}, need {_prestige.PrestigeCost})");
                return false;
            }

            if (!_prestige.CanPrestige(TotalCoinsEarned, Producers.Sum(p => p.Quantity), Upgrades.Sum(u => u.Level)))
            {
                Debug.WriteLine("Cannot prestige: not enough prestige points available");
                return false;
            }

            Coins = 0;
            _prestige.ApplyPrestige();
            Debug.WriteLine($"Prestiged to level {_prestige.PrestigeLevel}, new multiplier: {_prestige.PrestigeMultiplier}");
            return true;
        }

        public Prestige Prestige => _prestige;

        public double GlobalEfficiencyMultiplier
        {
            get => _globalEfficiencyMultiplier;
            set
            {
                if (_globalEfficiencyMultiplier != value)
                {
                    _globalEfficiencyMultiplier = value;
                    OnPropertyChanged();
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
                }
            }
        }

        public double TotalCoinsEarned
        {
            get => _totalCoinsEarned;
            internal set
            {
                if (_totalCoinsEarned != value)
                {
                    _totalCoinsEarned = value;
                    OnPropertyChanged();
                    CheckAchievements();
                }
            }
        }

        public TimeSpan TimePlayed
        {
            get => _timePlayed;
            private set
            {
                if (_timePlayed != value)
                {
                    _timePlayed = value;
                    OnPropertyChanged();
                }
            }
        }

        public void LoadFromSaveData(SaveData saveData)
        {
            Coins = saveData.Coins;
            MagicEssence = saveData.MagicEssence;
            ClickValue = saveData.ClickValue;
            PlayerName = saveData.PlayerName;
            CompanyName = saveData.CompanyName;
            TimePlayed = saveData.TimePlayed;
            TotalCoinsEarned = saveData.TotalCoinsEarned;
            GlobalEfficiencyMultiplier = saveData.GlobalEfficiencyMultiplier;
            CostReductionMultiplier = saveData.CostReductionMultiplier;
            SelectedBackground = saveData.SelectedBackground;
            SelectedSpecialization = saveData.SelectedSpecialization;
            PrestigeLevel = saveData.PrestigeLevel;

            _producers.Clear();
            foreach (var producerData in saveData.Producers)
            {
                var producer = new Producer(producerData.Name, producerData.Description, producerData.BaseCost, producerData.BaseProduction)
                {
                    Quantity = producerData.Quantity,
                    _efficiencyMultiplier = producerData.EfficiencyMultiplier,
                    _costReductionMultiplier = producerData.CostReductionMultiplier,
                    _quantityMultiplier = producerData.QuantityMultiplier
                };
                _producers.Add(producer);
            }

            _upgrades.Clear();
            foreach (var upgradeData in saveData.Upgrades)
            {
                var upgrade = new Upgrade(upgradeData.Name, upgradeData.Description, upgradeData.Cost, upgradeData.Effect, upgradeData.Type, upgradeData.TargetProducerName)
                {
                    IsPurchased = upgradeData.IsPurchased
                };
                _upgrades.Add(upgrade);
            }

            _achievements.Clear();
            foreach (var achievementData in saveData.Achievements)
            {
                var achievement = new Achievement(achievementData.Name, achievementData.Description, achievementData.RequiredValue, achievementData.Type, achievementData.Bonus, achievementData.TargetProducerName)
                {
                    IsUnlocked = achievementData.IsUnlocked,
                    Progress = achievementData.Progress,
                    UnlockTime = achievementData.UnlockTime
                };
                _achievements.Add(achievement);
            }

            _enhancements.Clear();
            foreach (var enhancementData in saveData.Enhancements)
            {
                var enhancement = new Enhancement(enhancementData.Name, enhancementData.Description, enhancementData.Cost, enhancementData.Effect, enhancementData.Type)
                {
                    IsPurchased = enhancementData.IsPurchased,
                    IsActive = enhancementData.IsActive
                };
                _enhancements.Add(enhancement);
            }

            // Restore enhancement references to producers
            foreach (var producerData in saveData.Producers)
            {
                var producer = _producers.FirstOrDefault(p => p.Name == producerData.Name);
                if (producer != null)
                {
                    foreach (var enhancementId in producerData.EnhancementIds)
                    {
                        var enhancement = _enhancements.FirstOrDefault(e => e.Name == enhancementId);
                        if (enhancement != null)
                        {
                            producer.Enhancements.Add(enhancement);
                        }
                    }
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
        }

        private void UpdateAchievementProgress(AchievementType type, double value, string? targetProducerName = null)
        {
            foreach (var achievement in Achievements.Where(a => a.Type == type && !a.IsUnlocked))
            {
                if (type == AchievementType.ProducerQuantity && achievement.TargetProducerName != targetProducerName)
                {
                    continue;
                }

                achievement.UpdateProgress(this);
                
                if (value >= achievement.RequiredValue)
                {
                    achievement.IsUnlocked = true;
                    ApplyAchievementReward(achievement);
                }
            }
        }

        private void ApplyAchievementReward(Achievement achievement)
        {
            switch (achievement.Type)
            {
                case AchievementType.CoinsEarned:
                    _globalEfficiencyMultiplier *= achievement.Bonus;
                    break;
                case AchievementType.TotalProducers:
                case AchievementType.ProducerQuantity:
                    _globalEfficiencyMultiplier *= achievement.Bonus;
                    break;
                case AchievementType.TotalUpgrades:
                    _costReductionMultiplier *= achievement.Bonus;
                    break;
                case AchievementType.ClickValue:
                    ClickValue *= achievement.Bonus;
                    break;
                case AchievementType.MagicEssence:
                    _globalEfficiencyMultiplier *= achievement.Bonus;
                    break;
                case AchievementType.TimePlayed:
                    _globalEfficiencyMultiplier *= achievement.Bonus;
                    break;
            }
            Debug.WriteLine($"Achievement reward applied: {achievement.Name}, Bonus: {achievement.Bonus}");
        }

        public bool TryPurchaseEnhancement(Producer producer, Enhancement enhancement)
        {
            if (producer == null || enhancement == null)
            {
                Debug.WriteLine("Cannot purchase enhancement: producer or enhancement is null");
                return false;
            }

            if (MagicEssence < enhancement.Cost)
            {
                Debug.WriteLine($"Cannot purchase enhancement: not enough Magic Essence (have {MagicEssence}, need {enhancement.Cost})");
                return false;
            }

            if (producer.Enhancements.Contains(enhancement))
            {
                Debug.WriteLine($"Cannot purchase enhancement: {enhancement.Name} is already applied to {producer.Name}");
                return false;
            }

            MagicEssence -= enhancement.Cost;
            producer.ApplyEnhancement(enhancement);
            Debug.WriteLine($"Purchased enhancement {enhancement.Name} for {producer.Name}");
            return true;
        }

        private void CheckAchievements()
        {
            // Implementation of CheckAchievements method
        }

        public void AddProducer(Producer producer)
        {
            if (producer != null)
            {
                _producers.Add(producer);
                OnPropertyChanged(nameof(Producers));
            }
        }

        public void AddUpgrade(Upgrade upgrade)
        {
            if (upgrade != null)
            {
                _upgrades.Add(upgrade);
                OnPropertyChanged(nameof(Upgrades));
            }
        }

        public void AddAchievement(Achievement achievement)
        {
            if (achievement != null)
            {
                _achievements.Add(achievement);
                OnPropertyChanged(nameof(Achievements));
            }
        }

        public void AddEnhancement(Enhancement enhancement)
        {
            if (enhancement == null)
            {
                throw new ArgumentNullException(nameof(enhancement));
            }

            if (!_enhancements.Contains(enhancement))
            {
                _enhancements.Add(enhancement);
                OnPropertyChanged(nameof(Enhancements));
                Debug.WriteLine($"Enhancement added to game state: {enhancement.Name}");
            }
        }

        public void ApplyEnhancementToProducer(Enhancement enhancement, Producer producer)
        {
            if (enhancement == null)
            {
                throw new ArgumentNullException(nameof(enhancement));
            }

            if (producer == null)
            {
                throw new ArgumentNullException(nameof(producer));
            }

            if (_enhancements.Contains(enhancement) && _producers.Contains(producer))
            {
                producer.ApplyEnhancement(enhancement);
                Debug.WriteLine($"Enhancement {enhancement.Name} applied to producer {producer.Name}");
            }
        }

        public void AddCoins(double amount)
        {
            if (amount > 0)
            {
                Coins += amount;
                TotalCoinsEarned += amount;
            }
        }

        public void AddMagicEssence(double amount)
        {
            if (amount > 0)
            {
                MagicEssence += amount;
            }
        }

        public void SaveGame()
        {
            var saveData = new SaveData
            {
                Coins = Coins,
                MagicEssence = MagicEssence,
                ClickValue = ClickValue,
                PlayerName = PlayerName,
                CompanyName = CompanyName,
                TimePlayed = TimePlayed,
                TotalCoinsEarned = TotalCoinsEarned,
                GlobalEfficiencyMultiplier = GlobalEfficiencyMultiplier,
                CostReductionMultiplier = CostReductionMultiplier,
                SelectedBackground = SelectedBackground,
                SelectedSpecialization = SelectedSpecialization,
                PrestigeLevel = PrestigeLevel,
                Producers = Producers.Select(p => new ProducerSaveData
                {
                    Name = p.Name,
                    Description = p.Description,
                    Quantity = p.Quantity,
                    BaseCost = p.BaseCost,
                    BaseProduction = p.BaseProduction,
                    EfficiencyMultiplier = p._efficiencyMultiplier,
                    CostReductionMultiplier = p._costReductionMultiplier,
                    QuantityMultiplier = p._quantityMultiplier,
                    EnhancementIds = p.Enhancements.Select(e => e.Name).ToList()
                }).ToList(),
                Upgrades = Upgrades.Select(u => new UpgradeSaveData
                {
                    Name = u.Name,
                    Description = u.Description,
                    Cost = u.BaseCost,
                    Effect = u.EffectValue,
                    Type = u.Type,
                    TargetProducerName = u.TargetProducerName,
                    IsPurchased = u.IsPurchased
                }).ToList(),
                Achievements = Achievements.Select(a => new AchievementSaveData
                {
                    Name = a.Name,
                    Description = a.Description,
                    RequiredValue = a.RequiredValue,
                    Type = a.Type,
                    Bonus = a.Bonus,
                    TargetProducerName = a.TargetProducerName,
                    IsUnlocked = a.IsUnlocked,
                    Progress = a.Progress,
                    UnlockTime = a.UnlockTime
                }).ToList(),
                Enhancements = Enhancements.Select(e => new EnhancementSaveData
                {
                    Name = e.Name,
                    Description = e.Description,
                    Cost = e.Cost,
                    Effect = e.Effect,
                    Type = e.Type,
                    IsPurchased = e.IsPurchased,
                    IsActive = e.IsActive
                }).ToList()
            };

            SaveManager.SaveGame(saveData);
        }

        public static GameState LoadGame()
        {
            var saveData = SaveManager.LoadGame();
            var gameState = new GameState();
            gameState.LoadFromSaveData(saveData);
            return gameState;
        }

        public string SelectedBackground { get; set; } = string.Empty;
        public string SelectedSpecialization { get; set; } = string.Empty;
        public int PrestigeLevel { get; private set; }
        public double PrestigeMultiplier => Math.Pow(1.1, PrestigeLevel);
        public bool CanPrestige => TotalCoinsEarned >= 1_000_000 * Math.Pow(10, PrestigeLevel);

        public void PerformPrestige()
        {
            if (CanPrestige)
            {
                PrestigeLevel++;
                Coins = 0;
                MagicEssence = 0;
                ClickValue = 1;
                foreach (var producer in Producers)
                {
                    producer.Quantity = 0;
                }
                foreach (var upgrade in Upgrades)
                {
                    upgrade.IsPurchased = false;
                }
                foreach (var enhancement in Enhancements)
                {
                    enhancement.IsPurchased = false;
                    enhancement.IsActive = false;
                }
                OnPropertyChanged(nameof(PrestigeLevel));
                OnPropertyChanged(nameof(PrestigeMultiplier));
                OnPropertyChanged(nameof(CanPrestige));
            }
        }

        public double CalculatePrestigePoints()
        {
            return Math.Floor(Math.Log10(TotalCoinsEarned / 1_000_000));
        }

        public bool PurchaseProducer(Producer producer)
        {
            if (producer == null)
            {
                throw new ArgumentNullException(nameof(producer));
            }

            if (Coins >= producer.CurrentCost)
            {
                Coins -= producer.CurrentCost;
                producer.Purchase();
                return true;
            }
            return false;
        }

        public bool PurchaseUpgrade(Upgrade upgrade)
        {
            if (upgrade == null)
            {
                throw new ArgumentNullException(nameof(upgrade));
            }

            if (Coins >= upgrade.Cost)
            {
                Coins -= upgrade.Cost;
                upgrade.Purchase();
                return true;
            }
            return false;
        }

        public bool PurchaseEnhancement(Enhancement enhancement)
        {
            if (enhancement == null)
            {
                throw new ArgumentNullException(nameof(enhancement));
            }

            if (MagicEssence >= enhancement.Cost)
            {
                MagicEssence -= enhancement.Cost;
                enhancement.Purchase();
                return true;
            }
            return false;
        }
    }
} 