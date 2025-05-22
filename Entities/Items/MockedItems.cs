
using System.Collections.Generic;

namespace MAPZ_lab_RPG.Entities.Items
{
    public class MockedItems
    {
        public static List<IItem> GetItems()
        {
            return new List<IItem>
            {
                new HealthRune(20.0),
                new DamageRune(5.0),
                new ArmorRune(2.5)
            };
        }
    }
}
