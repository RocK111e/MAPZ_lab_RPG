using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAPZ_lab_RPG.Entities.Items
{
    public class HealthRune : Item
    {
        public HealthRune(double health) : base("HealthRune", "Gives additional Health",
         health, 0.0, 0.0, 50)
        {
        }

        public override IItem Copy()
        {
            return new HealthRune(Health);
        }
    }
    public class DamageRune : Item
    {
        public DamageRune(double damage) : base("DamageRune", "Gives additional Damage",
         0.0, damage, 0.0, 50)
        {
        }

        public override IItem Copy()
        {
            return new DamageRune(Damage);
        }
    }
    public class ArmorRune : Item
    {
        public ArmorRune(double armor) : base("ArmorRune", "Gives additional Armor",
         0.0, 0.0, armor, 50)
        {
        }

        public override IItem Copy()
        {
            return new ArmorRune(Armor);
        }
    }
}