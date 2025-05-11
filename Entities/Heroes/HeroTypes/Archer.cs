namespace MAPZ_lab_RPG.Entities.Heroes.HeroTypes
{
    using MAPZ_lab_RPG.Entities.Heroes;
    using System;

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
            Inventory = new List<Items>();
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
            Perks++;
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
    }
}