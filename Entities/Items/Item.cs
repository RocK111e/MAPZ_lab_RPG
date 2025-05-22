
namespace MAPZ_lab_RPG.Entities.Items
{
    public abstract class Item : IItem
    {
        protected Item(string name, string description, double health, double damage, double armor, int price)
        {
            Name = name;
            Description = description;
            Health = health;
            Damage = damage;
            Armor = armor;
            Price = price;
        }

        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public double Health { get; protected set; }
        public double Damage { get; protected set; }
        public double Armor { get; protected set; }
        public int Price { get; protected set; }

        public abstract IItem Copy();
    }
}