using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAPZ_lab_RPG.Entities.Weapons
{
    public class Weapon
    {
        public Weapon(){
            Damage = 10;
            Name = "Knife";
        }
        public float Damage;
        public string Name;
    }
}