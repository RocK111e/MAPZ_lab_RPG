namespace MAPZ_lab_RPG.Entities.Heroes.HeroTypes
{
    using MAPZ_lab_RPG.Entities.Heroes;
    using System;

    public class Pirate : IHero
    {
        public Pirate()
        {
            MaxHealth = 125; // Middle HP (between Archer's 100 and Swordsman's 150)
            CurrentHealth = 125;
            Damage = 12; // Middle damage (between Archer's 15 and Swordsman's 10)
            Armor = 7; // Middle armor (between Archer's 5 and Swordsman's 10)
            Name = "Pirate";
            Coins = 5;
            Experience = 0;
            Level = 1;
            LevelPoints = 0;
            Inventory = new List<Items>();
        }

        public float Atack()
        {
            System.Console.Out.WriteLine("Pirate is slashing with cutlass");
            return Damage;
        }

        public float GetDamage(float damage)
        {
            damage = damage / (1 + (Armor / 100));
            CurrentHealth -= damage;
            System.Console.Out.WriteLine("Pirate is being attacked");
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

        public int AddCoins(int coins)
        {
            Coins += (int)(coins * 1.25);
            return Coins;
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
                    Damage += 4; // Middle damage increase (between Archer's 5 and Swordsman's 3)
                    break;
                case "Armor":
                    Armor += 2; // Middle armor increase (matches Archer, less than Swordsman's 3)
                    break;
                case "Health":
                    MaxHealth += 25; // Middle health increase (between Archer's 20 and Swordsman's 30)
                    break;
                default:
                    System.Console.Out.WriteLine("Invalid attribute");
                    break;
            }
        }
    }
}