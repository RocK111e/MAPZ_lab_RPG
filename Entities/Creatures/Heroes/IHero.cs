using System.Collections.Generic;
using MAPZ_lab_RPG.Entities.Items;
namespace MAPZ_lab_RPG.Entities.Heroes
{
    public interface IHero : ICreature
    {
        double MaxHealth { get; set; }
        int Coins { get; set; }
        int Experience { get; set; }
        int UpgradePoints { get; set; }
        int Level { get; set; }
        double Heal(double healAmount);
        void LevelUp();
        int AddCoins(int coins);
        int SpendCoins(int coins);
        void AddExperience(int experience);
        void Upgrade(string atribute);
        void AddItem(IItem item);
        bool RemoveItem(IItem item);
        List<IItem> GetItems();
        int GetItemsCount();
    }
}
