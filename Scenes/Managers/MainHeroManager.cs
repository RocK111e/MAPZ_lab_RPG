using Godot;
using MAPZ_lab_RPG.Entities.Heroes;

namespace Scenes.Managers;
class MainHeroManager
{
    private Node2D _heroDisplayNode;
    private MainHero _hero;

    private Label _heroNameLabel;
    private string _heroName; 
    private ProgressBar _heroHealthBar;
    private Label _heroHealthLabel;

    private Label _moneyLabelNode;
    private Label _levelLabelNode;

    public MainHeroManager(Node2D heroDisplayNode, string heroName, Label moneyLabel, Label levelLabel)
    {
        _heroDisplayNode = heroDisplayNode;
        _heroName = heroName;

        _heroNameLabel = heroDisplayNode.GetNode<Label>("Label");
        _heroHealthBar = heroDisplayNode.GetNode<ProgressBar>("ProgressBar");
        _heroHealthLabel = heroDisplayNode.GetNode<Label>("ProgressBar/HPLabel"); 
        _moneyLabelNode = moneyLabel;
        _levelLabelNode = levelLabel;

        _hero = MainHero.Instance;
        _hero.HeroSelect(heroName);

        _heroNameLabel.Text = _heroName;
        InitialUISetup();
    }

    private void InitialUISetup()
    {
        UpdateHealthUI();
        UpdateMoneyUI();
        UpdateLevelUI();
    }

    public void ApplyDamage(int damage)
    {
        _hero.TakeDamage(damage);
        UpdateHealthUI();        

        // TODO:
        // Optional: Add logic here if the hero's health drops to or below zero
        // e.g., if (_hero.GetCurrentHealth() <= 0) { HandleHeroDeath(); }
    }

    private void UpdateHealthUI()
    {
        double currentHealth = _hero.GetCurrentHealth();
        double maxHealth = _hero.GetMaxHealth();

        _heroHealthBar.MaxValue = maxHealth;
        _heroHealthBar.Value = currentHealth;
        
        _heroHealthLabel.Text = $"{currentHealth:0}/{maxHealth:0}"; 
    }

    private void UpdateMoneyUI()
    {
        if (_moneyLabelNode != null)
        {
            _moneyLabelNode.Text = $"Money: {_hero.GetCoins()}";
        }
    }

    private void UpdateLevelUI()
    {
        if (_levelLabelNode != null)
        {
            _levelLabelNode.Text = $"Level: {_hero.GetLevel()}";
        }
    }

    public void HealHero(double amount)
    {
        _hero.Heal(amount);
        UpdateHealthUI();
    }

    public void AddExperienceAndLevelUpCheck(int experience)
    {
        int previousLevel = _hero.GetLevel();
        _hero.AddExperience(experience); 
        
        if (_hero.GetLevel() > previousLevel)
        {
            UpdateLevelUI();
            UpdateHealthUI(); 
        }
    }
    
    public void ManuallyTriggerLevelUp() 
    {
        _hero.LevelUp();
        UpdateLevelUI();
        UpdateHealthUI();
    }

    public void GainCoins(int amount)
    {
        _hero.AddCoins(amount);
        UpdateMoneyUI();
    }

    public void SpendHeroCoins(int amount)
    {
        _hero.SpendCoins(amount);
        UpdateMoneyUI();
    }

    public void RefreshAllUI()
    {
        _heroNameLabel.Text = _heroName;
        UpdateHealthUI();
        UpdateMoneyUI();
        UpdateLevelUI();
    }
}