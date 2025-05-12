using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAPZ_lab_RPG.Entities.Items
{
    public class HealthRune : Item
    {
        public HealthRune(string name, string description, double health)
            : base(name, description, health, 0.0, 0.0)
        {}

        public override IItem Copy()
        {
            return new HealthRune(Name, Description, Damage);
        }
    }
    public class DamageRune : Item
    {
        public DamageRune(string name, string description, double damage)
            : base(name, description, 0.0, damage, 0.0)
        {}

        public override IItem Copy()
        {
            return new DamageRune(Name, Description, Damage);
        }
    }
    public class ArmorRune : Item
    {
        public ArmorRune(string name, string description, double armor)
            : base(name, description, 0.0, 0.0, armor)
        {
        }

        public override IItem Copy()
        {
            return new ArmorRune(Name, Description, Armor);
        }
    }
    public class ItemPrototypeManager
    {
        private static readonly ItemPrototypeManager _instance = new ItemPrototypeManager();
        private readonly Dictionary<string, IItem> _prototypes;

        public static ItemPrototypeManager Instance => _instance;

        private ItemPrototypeManager()
        {
            _prototypes = new Dictionary<string, IItem>
            {
                { "HealthRune", new HealthRune("Health Rune", "Gives additional Health", 20.0) },
                { "DamageRune", new DamageRune("Damage Rune", "Gives additional Damage", 5.0) },
                { "ArmorRune", new ArmorRune("Armor Rune", "Gives additional Armor", 2.5) }
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