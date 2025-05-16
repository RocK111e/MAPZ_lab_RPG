using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAPZ_lab_RPG.Entities.Items
{
    public class HealthRune : Item
    {
        public HealthRune(double health) : base("HealthRune", "Gives additional Health", health, 0.0, 0.0)
        {
        }

        public override IItem Copy()
        {
            return new HealthRune(Health);
        }
    }
    public class DamageRune : Item
    {
        public DamageRune(double damage) : base("DamageRune", "Gives additional Damage", 0.0, damage, 0.0)
        {
        }

        public override IItem Copy()
        {
            return new DamageRune(Damage);
        }
    }
    public class ArmorRune : Item
    {
        public ArmorRune(double armor) : base("ArmorRune", "Gives additional Armor", 0.0, 0.0, armor)
        {
        }

        public override IItem Copy()
        {
            return new ArmorRune(Armor);
        }
    }
    public class ItemManager
    {
        private static readonly ItemManager _instance = new ItemManager();
        private readonly Dictionary<string, IItem> _prototypes;

        public static ItemManager Instance => _instance;

        private ItemManager()
        {
            _prototypes = new Dictionary<string, IItem>
            {
                { "HealthRune", new HealthRune(20.0) },
                { "DamageRune", new DamageRune(5.0) },
                { "ArmorRune", new ArmorRune(2.5) }
            };
        }

        public IItem CreateItem(string prototypeName)
        {
            if (_prototypes.TryGetValue(prototypeName, out var prototype))
            {
                return prototype.Copy();
            }
            throw new ArgumentException($"Prototype '{prototypeName}' not found.");
        }
    }
}