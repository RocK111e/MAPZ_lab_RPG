using MAPZ_lab_RPG.Entities.Items;
using MAPZ_lab_RPG.Entities.Heroes;
using System;
using System.Text.Json;
using System.Collections.Generic;

namespace MAPZ_lab_RPG.Entities.Heroes.HeroTypes
{
    public class Hero : IHero
    {
        private delegate int CoinMultiply(int coins);
        private CoinMultiply coinMultiplyDelegate = coins => coins;
        public Hero(double health, double damage, double armor, string name, Dictionary<string, object> additional)
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
            Inventory = new List<IItem>();
            if (additional != null && additional.TryGetValue("CoinMultiply", out object coinMultiplyValue))
            {
                if (coinMultiplyValue is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Number)
                {
                    double multiplier = jsonElement.GetDouble();
                    coinMultiplyDelegate = coins => (int)(coins * multiplier);
                }
            }
        }
        public int AddCoins(int coins)
        {
            Coins += coinMultiplyDelegate(coins);
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
            double damage = damageTaken / (1 + (Armor / 100));
            Health -= damage;
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

        public void AddExperience(int experience)
        {
            //TODO normal level up (more experience with each level)
            Experience += experience;
            if (Experience >= 100)
            {
                LevelUp();
                Experience -= 100;
            }
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

        public void AddItem(IItem item){
            Inventory.Add(item);
        }
        public string Name { get; set; }
        public double MaxHealth { get; set; }
        public double Health { get; set; }
        public double Damage { get; set; }
        public double Armor { get; set; }
        public int Coins { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; }
        public int UpgradePoints{ get; set; }
        public List<IItem> Inventory { get; set; }
    }
}