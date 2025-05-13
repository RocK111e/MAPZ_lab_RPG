using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAPZ_lab_RPG.Entities.Enemies
{
    public class Enemy
    {
        public Enemy(){
            Health = 100;
            Damage = 10;
            Armor = 5;
            Race = "Ork";
        }
        public double Atack(){
            System.Console.Out.WriteLine("Enemy is atacking");
            return Damage;
        }
        public double TakeDamage(double damageTaken){
            damage = damageTaken / (1 + (Armor / 100));
            Health -= damage;
            System.Console.Out.WriteLine("Enemy is being atacked");
            return Health;
        }
        public double Health {get; set;}
        public double Damage {get; set;}
        public double Armor {get; set;}
        public string Race {get; set;}
    }
}