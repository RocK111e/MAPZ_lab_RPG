using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAPZ_lab_RPG.Entities.Heroes
{
    public class MainHero
    {
        private static MainHero _instance;

        public static MainHero Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MainHero();
                }
                return _instance;
            }
        }

        private MainHero()
        {
            Health = 100;
            Damage = 10;
            Armor = 5;
            Name = "Kamala";
        }

        public float Atack()
        {
            System.Console.Out.WriteLine("Hero is atacking");
            return Damage;
        }

        public float GetDamage(float damage)
        {
            Health -= damage;
            System.Console.Out.WriteLine("Hero is being atacked");
            return Health;
        }

        public float Health;
        public float Damage;
        public float Armor;
        public string Name;
    }

}