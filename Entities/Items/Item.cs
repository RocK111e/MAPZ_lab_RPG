
namespace MAPZ_lab_RPG.Entities.Items
{
    public class Item : IItem
    {
        public Item(string name,
                    string description,
                    double health,
                    double damage,
                    double armor,
                    int price)
        {
            Name = name;
            Description = description;
            Health = health;
            Damage = damage;
            Armor = armor;
            Price = price;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public double Health { get; set; }
        public double Damage { get; set; }
        public double Armor { get; set; }
        public int Price { get; set; }

        public IItem Copy()
        {
            return new Item(Name,
                            Description,
                            Health,
                            Damage,
                            Armor,
                            Price);
        }
    }
}