using MAPZ_lab_RPG.Entities.Items;
using MAPZ_lab_RPG.Entities.Heroes;
using System;
using System.Collections.Generic;

namespace MAPZ_lab_RPG.Entities.Heroes.HeroTypes
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
            Inventory = new List<IItem>();
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