using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;using MAPZ_lab_RPG.Entities.Items;
using MAPZ_lab_RPG.Entities.Heroes.HeroTypes;

namespace MAPZ_lab_RPG.Entities.Heroes
{
    public class MainHero
    {
        private readonly EntityReader entityReader;
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
            entityReader = new EntityReader();
        }
        public void HeroSelect(string heroName)
        {
            var heroData = entityReader.GetHeroData(heroName);
            hero = new Hero(heroData.Health, heroData.Damage, heroData.Armor, heroData.Name);
        }

        public List<string> GetHeroNames()
        {
            List<string> heroNames = entityReader.ReadHeroNames();
            return heroNames;
        }

        public double Atack()
        {
            return hero.Atack();
        }

        public double TakeDamage(double damageTaken)
        {
            return hero.TakeDamage(damageTaken);
        }
        public double Heal(double healAmount)
        {
            return hero.Heal(healAmount);
        }
        public void LevelUp()
        {
            hero.LevelUp();
        }
        public int AddCoins(int coins)
        {
            return hero.AddCoins(coins);
        }
        public int SpendCoins(int coins)
        {
            return hero.SpendCoins(coins);
        }
        public void AddExperience(int experience)
        {
            hero.AddExperience(experience);
        }
        public void Upgrade(string attribute)
        {
            hero.Upgrade(attribute);
        }
        public void AddItem(Item item)
        {
            hero.AddItem(item);
        }
        public double GetDamage()
        {
            return hero.Damage;
        }
        public double GetArmor()
        {
            return hero.Armor;
        }
        public double GetCurrentHealth()
        {
            return hero.Health;
        }
        public double GetMaxHealth()
        {
            return hero.MaxHealth;
        }
        public string GetName()
        {
            return hero.Name;
        }
        public int GetCoins()
        {
            return hero.Coins;
        }
        public int GetExperience()
        {
            return hero.Experience;
        }
        public int GetLevel()
        {
            return hero.Level;
        }
        public int GetUpgradePoints()
        {
            return hero.UpgradePoints;
        }
        public List<IItem> GetInventory()
        {
            return hero.Inventory;
        }
        private IHero hero { get; set; }
    }

}