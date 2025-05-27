using MAPZ_lab_RPG.Entities.Items;
using System.Collections.Generic;

namespace MAPZ_lab_RPG.Entities.Heroes
{
    public class Hero : IHero
    {
        public Hero(double health, double damage, double armor, string name)
        {
            MaxHealth = health;
            Health = health;
            Damage = damage;
            Armor = armor;
            Name = name;
            Coins = 0;
            Experience = 0;
            Level = 1;
            UpgradePoints = 0;
            _inventory = new Inventory();
        }
        public int AddCoins(int coins)
        {
            Coins += coins;
            return Coins;
        }

        public int SpendCoins(int coins)
        {
            Coins -= coins;
            return Coins;
        }

        public double Attack()
        {
            return Damage;
        }

        public double TakeDamage(double damageTaken)
        {
            double damage = damageTaken;
            double damageReducedArmor = damage / (1 + (Armor / 100));
            Health -= damageReducedArmor;
            return Health;
        }

        public double Heal(double healAmount)
        {
            Health += healAmount;
            if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }
            return Health;
        }

        public void AddExpirience(int expirience)
        {
            Experience += expirience;
            int expirienceToLevelUp = 100 + 50 * Level;
            if (Experience >= expirienceToLevelUp)
            {
                LevelUp();
                Experience -= expirienceToLevelUp;
            }
        }

        public void EarnRoundRewards(int coins, int expirience)
        {
            AddCoins(coins);
            AddExpirience(expirience);
        }

        public void LevelUp()
        {
            Level++;
            UpgradePoints++;
        }

        public void Upgrade(string attribute)
        {
            switch (attribute)
            {
                case "Damage":
                    Damage += 5;
                    break;
                case "Armor":
                    Armor += 2;
                    break;
                case "Health":
                    MaxHealth += 20;
                    break;
                default:
                    System.Console.Out.WriteLine("Invalid attribute");
                    break;
            }
        }
        public void AddItem(IItem item)
        {
            _inventory.AddItem(item);

            double ratio = Health / MaxHealth;
            MaxHealth += item.Health;
            Health = MaxHealth * ratio;

            Armor += item.Armor;
            Damage += item.Damage;
        }
        public bool RemoveItem(IItem item)
        {
            if (!_inventory.RemoveItem(item))
            {
                return false;
            }

            double ratio = Health / MaxHealth;
            MaxHealth -= item.Health;
            Health = MaxHealth * ratio;

            Armor -= item.Armor;
            Damage -= item.Damage;
            return true;
        }
        public List<IItem> GetItems()
        {
            return _inventory.GetItems();
        }
        public int GetItemsCount()
        {
            return _inventory.GetItemsCount();
        }
        public string Name { get; set; }
        public double Health { get; set; }
        public double MaxHealth { get; set; }
        public double Damage { get; set; }
        public double Armor { get; set; }
        public int Coins { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; }
        public int UpgradePoints{ get; set; }
        private Inventory _inventory;
    }
}