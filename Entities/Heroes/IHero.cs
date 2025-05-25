using System.Collections.Generic;
using MAPZ_lab_RPG.Entities.Items;
namespace MAPZ_lab_RPG.Entities.Heroes{
        
    interface IHero : IEntity
    {
        double Heal(double healAmount);
        void LevelUp();
        double MaxHealth { get; set; }
        int Coins { get; set; }
        int AddCoins(int coins);
        int SpendCoins(int coins);
        int Experience { get; set; }
        void AddExperience(int experience);
        int Level { get; set; }
        int UpgradePoints{ get; set; }
        void Upgrade(string atribute);
        List<IItem> Inventory { get; set; }
        void AddItem(IItem item);
    }
}
