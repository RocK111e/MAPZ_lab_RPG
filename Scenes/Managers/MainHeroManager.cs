using Godot;
using MAPZ_lab_RPG.Entities.Heroes; // Your MainHero class namespace
using System; // For Math.Max

namespace Scenes.Managers
{
    public class MainHeroManager
    {
        private Control _heroDisplayNode; // Root of Entity.tscn instance for hero
        private MainHero _hero; // Instance of your MainHero singleton

        // UI Elements within the heroDisplayNode
        private Label _heroNameLabel;
        private ProgressBar _heroHealthBar;
        private Label _heroHealthLabel;
        // private TextureRect _heroSprite; // If you need to access/change the TextureRect

        // External UI Elements
        private Label _moneyLabelNode;
        private Label _levelLabelNode;

        public MainHeroManager(Control heroDisplayNode, string heroName, Label moneyLabel, Label levelLabel)
        {
            if (heroDisplayNode == null) throw new ArgumentNullException(nameof(heroDisplayNode));
            _heroDisplayNode = heroDisplayNode;

            _heroNameLabel = _heroDisplayNode.GetNode<Label>("VBoxContainer/CenterContainer/Label");
            _heroHealthBar = _heroDisplayNode.GetNode<ProgressBar>("VBoxContainer/CenterContainer3/ProgressBar");
            _heroHealthLabel = _heroDisplayNode.GetNode<Label>("VBoxContainer/CenterContainer3/ProgressBar/HPLabel");
            // _heroSprite = _heroDisplayNode.GetNode<TextureRect>("VBoxContainer/CenterContainer2/TextureRect");

            if (_heroNameLabel == null || _heroHealthBar == null || _heroHealthLabel == null)
            {
                GD.PrintErr("MainHeroManager: Critical UI elements (Name, HealthBar, HPLabel) not found in heroDisplayNode. Check paths relative to Entity.tscn root (Control)!");
            }

            _moneyLabelNode = moneyLabel ?? throw new ArgumentNullException(nameof(moneyLabel));
            _levelLabelNode = levelLabel ?? throw new ArgumentNullException(nameof(levelLabel));

            _hero = MainHero.Instance;
            _hero.HeroSelect(heroName);

            if (_heroNameLabel != null) _heroNameLabel.Text = _hero.GetName();
            InitialUISetup();
        }

        public void SetNodes(Control heroDisplayNode, Label moneyLabel, Label levelLabel)
        {
            if (heroDisplayNode == null) throw new ArgumentNullException(nameof(heroDisplayNode));
            _heroDisplayNode = heroDisplayNode;

            _heroNameLabel = _heroDisplayNode.GetNode<Label>("VBoxContainer/CenterContainer/Label");
            _heroHealthBar = _heroDisplayNode.GetNode<ProgressBar>("VBoxContainer/CenterContainer3/ProgressBar");
            _heroHealthLabel = _heroDisplayNode.GetNode<Label>("VBoxContainer/CenterContainer3/ProgressBar/HPLabel");
            // _heroSprite = _heroDisplayNode.GetNode<TextureRect>("VBoxContainer/CenterContainer2/TextureRect");

            if (_heroNameLabel == null || _heroHealthBar == null || _heroHealthLabel == null)
            {
                GD.PrintErr("MainHeroManager: Critical UI elements (Name, HealthBar, HPLabel) not found in heroDisplayNode. Check paths relative to Entity.tscn root (Control)!");
            }

            _moneyLabelNode = moneyLabel ?? throw new ArgumentNullException(nameof(moneyLabel));
            _levelLabelNode = levelLabel ?? throw new ArgumentNullException(nameof(levelLabel));

            if (_heroNameLabel != null) _heroNameLabel.Text = _hero.GetName();
            InitialUISetup();
        }

        private void InitialUISetup()
        {
            UpdateHealthUI();
            UpdateMoneyUI();
            UpdateLevelUI();
        }

        private void UpdateHealthUI()
        {
            if (_hero == null || _heroHealthBar == null || _heroHealthLabel == null) return;
            double currentHealth = _hero.GetCurrentHealth();
            double maxHealth = _hero.GetMaxHealth();
            _heroHealthBar.MaxValue = maxHealth;
            _heroHealthBar.Value = Math.Max(0, currentHealth);
            _heroHealthLabel.Text = $"{_heroHealthBar.Value:F0}/{maxHealth:F0}";
        }

        private void UpdateMoneyUI()
        {
            if (_hero == null || _moneyLabelNode == null) return;
            _moneyLabelNode.Text = $"Money: {_hero.GetCoins()}";
        }

        private void UpdateLevelUI()
        {
            if (_hero == null || _levelLabelNode == null) return;
            _levelLabelNode.Text = $"Level: {_hero.GetLevel()}";
        }

        public void HealHero(double amount)
        {
            if (_hero == null) return;
            _hero.Heal(amount);
            UpdateHealthUI();
        }

        public void AddExperienceAndLevelUpCheck(int experience)
        {
            if (_hero == null) return;
            int previousLevel = _hero.GetLevel();
            _hero.AddExperience(experience);
            if (_hero.GetLevel() > previousLevel)
            {
                UpdateLevelUI();
                UpdateHealthUI();
            }
        }

        public void GainCoins(int amount)
        {
            if (_hero == null) return;
            _hero.AddCoins(amount);
            UpdateMoneyUI();
        }

        public void SpendHeroCoins(int amount)
        {
            if (_hero == null) return;
            _hero.SpendCoins(amount);
            UpdateMoneyUI();
        }

        public double GetHeroAttackDamage()
        {
            if (_hero == null) { GD.PrintErr("MainHeroManager: Hero instance null."); return 0; }
            return _hero.Atack();
        }

        public void HeroTakeDamage(double damageTaken)
        {
            if (_hero == null) { GD.PrintErr("MainHeroManager: Hero instance null."); return; }
            _hero.TakeDamage(damageTaken);
            UpdateHealthUI();
            if (!IsHeroAlive())
            {
                GD.Print($"{_hero.GetName()} has been defeated!");
            }
        }

        public bool IsHeroAlive()
        {
            if (_hero == null) return false;
            return _hero.GetCurrentHealth() > 0;
        }
    }
}