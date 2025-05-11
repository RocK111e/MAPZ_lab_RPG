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
        public float Atack(){
            System.Console.Out.WriteLine("Enemy is atacking");
            return Damage;
        }
        public float GetDamage(float Damage){
            Health = Health - Damage;
            System.Console.Out.WriteLine("Enemy is being atacked");
            return Health;
        }
        public float Health;
        public float Damage;
        public float Armor;
        public string Race;
    }
}