using MAPZ_lab_RPG.Entities.Items;
using System.Collections.Generic;

namespace MAPZ_lab_RPG.Entities.Heroes.Decorators
{
    public abstract class HeroBaseDecorator : IHero
    {
        protected readonly IHero _hero;

        protected HeroBaseDecorator(IHero hero)
        {
            _hero = hero;
        }

        // IHero-specific properties
        public virtual double MaxHealth
        {
            get => _hero.MaxHealth;
            set => _hero.MaxHealth = value;
        }

        public virtual int Coins
        {
            get => _hero.Coins;
            set => _hero.Coins = value;
        }

        public virtual int Experience
        {
            get => _hero.Experience;
            set => _hero.Experience = value;
        }

        public virtual int UpgradePoints
        {
            get => _hero.UpgradePoints;
            set => _hero.UpgradePoints = value;
        }

        public virtual int Level
        {
            get => _hero.Level;
            set => _hero.Level = value;
        }
        public virtual double Heal(double healAmount) => _hero.Heal(healAmount);
        public virtual void LevelUp() => _hero.LevelUp();
        public virtual int AddCoins(int coins) => _hero.AddCoins(coins);
        public virtual void AddExpirience(int expirience) => _hero.AddExpirience(expirience);
        public virtual int SpendCoins(int coins) => _hero.SpendCoins(coins);
        public virtual void EarnRoundRewards(int coins, int expirience) => _hero.EarnRoundRewards(coins, expirience);
        public virtual void Upgrade(string attribute) => _hero.Upgrade(attribute);
        public virtual void AddItem(IItem item) => _hero.AddItem(item);
        public virtual bool RemoveItem(IItem item) => _hero.RemoveItem(item);
        public virtual List<IItem> GetItems() => _hero.GetItems();
        public virtual int GetItemsCount() => _hero.GetItemsCount();

        // IEntity properties
        public virtual double Health
        {
            get => _hero.Health;
            set => _hero.Health = value;
        }

        public virtual double Damage
        {
            get => _hero.Damage;
            set => _hero.Damage = value;
        }

        public virtual double Armor
        {
            get => _hero.Armor;
            set => _hero.Armor = value;
        }

        public virtual string Name
        {
            get => _hero.Name;
            set => _hero.Name = value;
        }

        public virtual double Attack() => _hero.Attack();
        public virtual double TakeDamage(double damageTaken) => _hero.TakeDamage(damageTaken);
    }
}