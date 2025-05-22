using Godot;
using MAPZ_lab_RPG.Entities.Heroes; // Your MainHero class namespace
using System; // For Math.Max

namespace Scenes.Managers
{
    public class MainHeroManager // Made public
    {
        private Node2D _heroDisplayNode;
        private MainHero _hero; // Instance of your MainHero singleton

        private Label _heroNameLabel;
        // string _heroName was used as a fallback, now we will rely on _hero.GetName()
        private ProgressBar _heroHealthBar;
        private Label _heroHealthLabel;

        private Label _moneyLabelNode;
        private Label _levelLabelNode;

        public MainHeroManager(Node2D heroDisplayNode, string heroName, Label moneyLabel, Label levelLabel)
        {
            _heroDisplayNode = heroDisplayNode;

            _heroNameLabel = heroDisplayNode.GetNode<Label>("Label");
            _heroHealthBar = heroDisplayNode.GetNode<ProgressBar>("ProgressBar");
            _heroHealthLabel = heroDisplayNode.GetNode<Label>("ProgressBar/HPLabel");
            _moneyLabelNode = moneyLabel;
            _levelLabelNode = levelLabel;

            _hero = MainHero.Instance;
            _hero.HeroSelect(heroName); // Selects the hero type within MainHero

            _heroNameLabel.Text = _hero.GetName(); // Use GetName() from MainHero
            InitialUISetup();
        }

        private void InitialUISetup()
        {
            UpdateHealthUI();
            UpdateMoneyUI();
            UpdateLevelUI();
        }

        // Existing ApplyDamage, uses MainHero.TakeDamage
        public void ApplyDamage(int damage)
        {
            if (_hero == null) return;
            // MainHero.TakeDamage expects a double, so cast
            _hero.TakeDamage((double)damage);
            UpdateHealthUI();

            if (!IsHeroAlive())
            {
                GD.Print($"{_hero.GetName()} has been defeated!");
            }
        }

        private void UpdateHealthUI()
        {
            if (_hero == null || _heroHealthBar == null || _heroHealthLabel == null) return;

            // Use getter methods from MainHero
            double currentHealth = _hero.GetCurrentHealth();
            double maxHealth = _hero.GetMaxHealth();

            _heroHealthBar.MaxValue = maxHealth;
            _heroHealthBar.Value = Math.Max(0, currentHealth); // Ensure bar doesn't go below 0

            _heroHealthLabel.Text = $"{_heroHealthBar.Value:F0}/{maxHealth:F0}";
        }

        private void UpdateMoneyUI()
        {
            if (_hero == null || _moneyLabelNode == null) return;
            // Use getter method from MainHero
            _moneyLabelNode.Text = $"Money: {_hero.GetCoins()}";
        }

        private void UpdateLevelUI()
        {
            if (_hero == null || _levelLabelNode == null) return;
            // Use getter method from MainHero
            _levelLabelNode.Text = $"Level: {_hero.GetLevel()}";
        }

        // Existing methods, should already align with MainHero's public methods
        public void HealHero(double amount)
        {
            if (_hero == null) return;
            _hero.Heal(amount);
            UpdateHealthUI();
        }

        public void AddExperienceAndLevelUpCheck(int experience)
        {
            if (_hero == null) return;
            int previousLevel = _hero.GetLevel(); // Use getter
            _hero.AddExperience(experience);

            if (_hero.GetLevel() > previousLevel) // Use getter
            {
                UpdateLevelUI();
                UpdateHealthUI();
            }
        }

        public void ManuallyTriggerLevelUp()
        {
            if (_hero == null) return;
            _hero.LevelUp();
            UpdateLevelUI();
            UpdateHealthUI();
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

        public void RefreshAllUI()
        {
            if (_hero == null || _heroNameLabel == null) return;
            _heroNameLabel.Text = _hero.GetName(); // Use getter
            UpdateHealthUI();
            UpdateMoneyUI();
            UpdateLevelUI();
        }

        // --- METHODS NEEDED BY BATTLESCENE (adjusted to use MainHero's public API) ---

        /// <summary>
        /// Gets the hero's current attack damage using MainHero.Atack().
        /// </summary>
        /// <returns>The hero's attack damage as a double.</returns>
        public double GetHeroAttackDamage()
        {
            if (_hero == null)
            {
                GD.PrintErr("MainHeroManager: Hero instance is null in GetHeroAttackDamage.");
                return 0;
            }
            // MainHero.Atack() directly returns the damage of the internal IHero
            return _hero.Atack();
        }

        /// <summary>
        /// Applies damage to the hero from combat using MainHero.TakeDamage(double).
        /// </summary>
        /// <param name="damageTaken">The amount of damage to apply (as a double).</param>
        public void HeroTakeDamage(double damageTaken)
        {
            if (_hero == null)
            {
                GD.PrintErr("MainHeroManager: Hero instance is null in HeroTakeDamage.");
                return;
            }
            // MainHero.TakeDamage(double) handles the damage logic for the internal IHero
            _hero.TakeDamage(damageTaken);
            UpdateHealthUI();

            if (!IsHeroAlive())
            {
                GD.Print($"{_hero.GetName()} has been defeated!");
            }
        }

        /// <summary>
        /// Checks if the hero is still alive using MainHero.GetCurrentHealth().
        /// </summary>
        /// <returns>True if hero's current health is greater than 0, false otherwise.</returns>
        public bool IsHeroAlive()
        {
            if (_hero == null)
            {
                GD.PrintErr("MainHeroManager: Hero instance is null in IsHeroAlive.");
                return false;
            }
            // Uses the getter from MainHero
            return _hero.GetCurrentHealth() > 0;
        }
    }
}