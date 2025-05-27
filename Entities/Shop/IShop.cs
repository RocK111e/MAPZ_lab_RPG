
using MAPZ_lab_RPG.Entities.Items;

namespace MAPZ_lab_RPG.Entities.Shop
{
    public interface ShopInterface
    {
        public IItem BuyItem(string itemName, ref int heroBalance);
    }
}