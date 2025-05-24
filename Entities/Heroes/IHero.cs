using System.Collections.Generic;
using MAPZ_lab_RPG.Entities.Items;
namespace MAPZ_lab_RPG.Entities.Heroes{
        
    interface IHero : IEntity
    {
        public double Atack()
        {
            return Damage;
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
        public void LevelUp()
        {
            Level++;
            UpgradePoints++;
        }
        public double MaxHealth { get; set; }
        public int Coins { get; set; }
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
        public List<IItem> Inventory { get; set; }
        public void AddItem(IItem item){
            Inventory.Add(item);
        }
    }
}
