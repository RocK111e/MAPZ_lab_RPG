using System.Collections.Generic;
using MAPZ_lab_RPG.Entities.Items;
using MAPZ_lab_RPG.Entities.Heroes;

namespace MAPZ_lab_RPG.Entities.Shop
{
    public class Shop
    {
        private static readonly Shop _instance = new Shop();
        private readonly ItemManager itemManager;
        private List<IItem> items;

        public static Shop Instance
        {
            get { return _instance; }
        }

        private Shop()
        {
            itemManager = ItemManager.Instance;
        }
        public bool BuyItem(IItem item, ref MainHero hero)
        {
            if (hero.GetCoins() < item.Price)
            {
                return false;
            }

            bool success = items.Remove(item);

            if (!success)
            {
                return false;
            }

            hero.SpendCoins(item.Price);
            hero.AddItem(item);
            return true;
        }
        public bool SellItem(IItem item, ref MainHero hero)
        {
            if (!hero.RemoveItem(item))
            {
                return false;
            }
            
            items.Add(item);

            hero.AddCoins((int)(item.Price * 0.9));
            return true;
        }
        public List<IItem> GenerateItems()
        {
            items = itemManager.GenerateItems();

            return items;
        }
    }
}