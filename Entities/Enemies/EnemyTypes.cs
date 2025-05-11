using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAPZ_lab_RPG.Entities.Enemies
{
    //Tank
    public class Ork : IEnemy
    {
        public Ork(int level){
            Health = 200 + 20 * level;
            Damage = 5 + 0.5 * level;
            Armor = 8 + 1 * level;
            Race = "Ork";
        }
        public double Atack(){
            System.Console.Out.WriteLine("Ork is atacking");
            return Damage;
        }
        public double GetDamage(double Damage){
            Health = Health - Damage;
            System.Console.Out.WriteLine("Ork is being atacked");
            return Health;
        }
        public double Health { get; private set; }
        public double Damage { get; private set; }
        public double Armor { get; private set; }
        public string Race { get; private set; }
    }
    //Damager
    public class Goblin : IEnemy
    {
        public Goblin(int level){
            Health = 75 + 10 * level;
            Damage = 15 + 1.5 * level;
            Armor = 3 + 0.5 * level;
            Race = "Goblin";
        }
        public double Atack(){
            System.Console.Out.WriteLine("Goblin is atacking");
            return Damage;
        }
        public double GetDamage(double Damage){
            Health = Health - Damage;
            System.Console.Out.WriteLine("Goblin is being atacked");
            return Health;
        }
        public double Health { get; private set; }
        public double Damage { get; private set; }
        public double Armor { get; private set; }
        public string Race { get; private set; }
    }
    //Normis
    public class Trol : IEnemy
    {
        public Trol(int level){
            Health = 100 + 15 * level;
            Damage = 10 + 1 * level;
            Armor = 5 + 1 * level;
            Race = "Trol";
        }
        public double Atack(){
            System.Console.Out.WriteLine("Trol is atacking");
            return Damage;
        }
        public double GetDamage(double Damage){
            Health = Health - Damage;
            System.Console.Out.WriteLine("Trol is being atacked");
            return Health;
        }
        public double Health { get; private set; }
        public double Damage { get; private set; }
        public double Armor { get; private set; }
        public string Race { get; private set; }
    }
}