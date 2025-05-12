using System.Collections.Generic;
using MAPZ_lab_RPG.Entities.Items;
namespace MAPZ_lab_RPG.Entities.Heroes{
        
    interface IHero
    {
        public string Name { get; set; }
        public float Atack();
        public float GetDamage(float Damage);
        public float Heal(float HealAmount);
        public void LevelUp();
        public float MaxHealth { get; set; }
        public float CurrentHealth { get; set; }
        public float Damage { get; set; }
        public float Armor { get; set; }

        public int Coins { get; set; }
        public int AddCoins(int coins)
        {
            Coins += coins;
            return Coins;
        }
        public int Experience { get; set; }
        public void AddExperience(int experience)
        {
            Experience += experience;
            if (Experience >= 100)
            {
                LevelUp();
                Experience -= 100;
            }
        }
        public int Level { get; set; }
        public int LevelPoints{ get; set; }
        public void Upgrade(string atribute);
        public List<Item> Inventory { get; set; }
        public void AddItem(Item item){
            Inventory.Add(item);
        }
}

}
