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
        }

        public HeroSelect(string hero_name){
            switch (hero_name)
            {
                case "Archer":
                    hero = new Archer();
                    break;
                case "Swordsman":
                    hero = new Swordsman();
                    break;
                case "Pirate":
                    hero = new Pirate();
                    break;
                default:
                    throw new ArgumentException("Invalid hero name");
            }
        }

        public float Atack()
        {
            return hero.Atack();
        }

        public float GetDamage(float damage)
        {
            return hero.GetDamage(damage);
        }
        public float Heal(float healAmount)
        {
            return hero.Heal(healAmount);
        }
        public void LevelUp()
        {
            hero.LevelUp();
        }
        public int AddCoins(float coins)
        {
            return hero.AddCoins(coins);
        }
        public void AddExperience(int experience)
        {
            hero.AddExperience(experience);
        }
        public void Upgrade(string attribute)
        {
            hero.Upgrade(attribute);
        }
        public void AddItem(Items item)
        {
            hero.AddItem(item);
        }
        private IHero hero {get; set;};
    }

}