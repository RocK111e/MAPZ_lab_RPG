using System;
using System.Collections.Generic;
namespace MAPZ_lab_RPG.Entities.Items
{
    public class ItemManager
    {
        private readonly int itemCount = 4;
        private static readonly ItemManager _instance = new ItemManager();
        private readonly List<IItem> items;

        public static ItemManager Instance => _instance;

        private ItemManager()
        {
            items = MockedItems.GetItems();
        }

        public List<IItem> GenerateItems()
        {
            List<IItem> itemList = new List<IItem>();

            Random rand = new Random();

            for (int i = 0; i < itemCount; i++)
            {
                int choice = rand.Next(0, items.Count);
                itemList.Add(items[choice]);
            }
            
            return itemList;
        }
    }
}