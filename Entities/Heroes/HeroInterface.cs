using System.Collections.Generic;
using MAPZ_lab_RPG.Entities.Items;
namespace MAPZ_lab_RPG.Entities.Heroes{
        
    interface IHero
    {
        public string Name { get; set; }
        public double Atack()
        {
            return Damage;
        }
        public double TakeDamage(double damageTaken)
        {
            damage = damageTaken / (1 + (Armor / 100));
            CurrentHealth -= damage;
            return CurrentHealth;
        }
        public double Heal(double HealAmount)
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
            UpgradePoints++;
        }
        public double MaxHealth { get; set; }
        public double CurrentHealth { get; set; }
        public double Damage { get; set; }
        public double Armor { get; set; }

        public int Coins { get; set; }
        public int AddCoins(int coins)
        {
            Coins += coins;
            return Coins;
        }
        public int Experience { get; set; }
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
        public int Level { get; set; }
        public int UpgradePoints{ get; set; }
        public void Upgrade(string atribute);
        public List<Item> Inventory { get; set; }
        public void AddItem(Item item){
            Inventory.Add(item);
        }
    }
}
