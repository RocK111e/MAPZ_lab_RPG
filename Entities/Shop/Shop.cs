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
        public IItem BuyItem(IItem item, ref MainHero hero)
        {
            if (hero.GetCoins() < item.Price){
                return null;
            }
            
            bool success = items.Remove(item);

            if (!success){
                return null;
            }

            hero.SpendCoins(item.Price);
            return item;
        }
        public List<IItem> GenerateItems()
        {
            items = itemManager.GenerateItems();

            return items;
        }
    }
}