using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAPZ_lab_RPG.Entities.Items
{
    public interface IItem
    {
        string Name { get; }
        string Description { get; }
        double Health { get; }
        double Damage { get; }
        double Armor { get; }
        IItem Copy();
    }
    public abstract class Item : IItem
    {
        protected Item(string name, string description, double health, double damage, double armor)
        {
            Name = name;
            Description = description;
            Health = health;
            Damage = damage;
            Armor = armor;
        }

        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public double Health { get; protected set; }
        public double Damage { get; protected set; }
        public double Armor { get; protected set; }
        public abstract IItem Copy();
    }
}