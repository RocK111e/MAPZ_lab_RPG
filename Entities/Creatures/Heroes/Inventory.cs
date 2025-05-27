using System.Collections.Generic;
using MAPZ_lab_RPG.Entities.Items;

namespace MAPZ_lab_RPG.Entities.Heroes
{
    public class Inventory
    {
        private List<IItem> _inventory;
        public Inventory()
        {
            _inventory = new List<IItem>();
        }
        // public double GetBonusHealth()
        // {
        //     double health = 0;

        //     foreach (IItem item in _inventory)
        //     {
        //         if (item.Health > 0)
        //         {
        //             health += item.Health;
        //         }
        //     }

        //     return health;
        // }
        // public double GetBonusArmor()
        // {
        //     double armor = 0;

        //     foreach (IItem item in _inventory)
        //     {
        //         if (item.Armor > 0)
        //         {
        //             armor += item.Armor;
        //         }
        //     }

        //     return armor;
        // }
        // public double GetBonusDamage()
        // {
        //     double damage = 0;

        //     foreach (IItem item in _inventory)
        //     {
        //         if (item.Damage > 0)
        //         {
        //             damage += item.Damage;
        //         }
        //     }

        //     return damage;
        // }
        public int GetItemsCount()
        {
            return _inventory.Count;
        }
        public List<IItem> GetItems()
        {
            return _inventory;
        }
        public void AddItem(IItem item)
        {
            _inventory.Add(item);
        }
        public void RemoveItem(IItem item)
        {
            _inventory.Remove(item);
        }
    }
}