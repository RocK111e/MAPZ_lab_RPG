using MAPZ_lab_RPG.Entities.Items;
using MAPZ_lab_RPG.Entities.Heroes;
using System;
using System.Collections.Generic;

namespace MAPZ_lab_RPG.Entities.Heroes.HeroTypes
{

    public class Archer : IHero
    {
        public Archer()
        {
            MaxHealth = 100;
            CurrentHealth = 100;
            Damage = 15;
            Armor = 5;
            Name = "Archer";
            Coins = 0;
            Experience = 0;
            Level = 1;
            LevelPoints = 0;
            Inventory = new List<Item>();
        }

        public float Atack()
        {
            System.Console.Out.WriteLine("Archer is shooting");
            return Damage;
        }

        public float GetDamage(float damage)
        {
            damage = damage / (1 + (Armor / 100));
            CurrentHealth -= damage;
            System.Console.Out.WriteLine("Archer is being atacked");
            return CurrentHealth;
        }

        public float Heal(float healAmount)
        {
            CurrentHealth += healAmount;
            if (CurrentHealth > MaxHealth)
            {
                CurrentHealth = MaxHealth;
            }
            return CurrentHealth;
        }
        public void LevelUp()
        {
            Level++;
            LevelPoints++;
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
        public float MaxHealth { get; set; }
        public float CurrentHealth { get; set; }
        public float Damage { get; set; }
        public float Armor { get; set; }

        public int Coins { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; }
        public int LevelPoints{ get; set; }
        public List<Item> Inventory { get; set; }
    }
}