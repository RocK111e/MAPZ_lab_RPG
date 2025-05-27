using System.Collections.Generic;
using MAPZ_lab_RPG.Entities.Items;
using MAPZ_lab_RPG.Entities.Enemies;

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
        public void HeroSelect(string heroName)
        {
			_hero = EntityCreator.Instance.CreateHero(heroName);
		}

		public List<string> GetHeroNames()
		{
			List<string> heroNames = EntityReader.Instance.ReadHeroNames();
			return heroNames;
		}

		public double Atack()
		{
			return _hero.Attack();
		}

		public double TakeDamage(double damageTaken)
		{
			return _hero.TakeDamage(damageTaken);
		}
		public double Heal(double healAmount)
		{
			return _hero.Heal(healAmount);
		}
		public void LevelUp()
		{
			_hero.LevelUp();
		}
		public int AddCoins(int coins)
		{
			return _hero.AddCoins(coins);
		}
		public void AddExpirience(int expirience)
		{
			_hero.AddExpirience(expirience);
		}
		public int SpendCoins(int coins)
		{
			return _hero.SpendCoins(coins);
		}
		public void EarnRoundRewards(int round)
		{
			int coinsPerRound = 50 + 5 * round;
            int expiriencePerRound = 30 + 20 * round;
			_hero.EarnRoundRewards(coinsPerRound, expiriencePerRound);
		}
		public void Upgrade(string attribute)
		{
			_hero.Upgrade(attribute);
		}
		public double GetDamage()
		{
			return _hero.Damage;
		}
		public double GetArmor()
		{
			return _hero.Armor;
		}
		public double GetCurrentHealth()
		{
			return _hero.Health;
		}
		public double GetMaxHealth()
		{
			return _hero.MaxHealth;
		}
		public string GetName()
		{
			return _hero.Name;
		}
		public int GetCoins()
		{
			return _hero.Coins;
		}
		public int GetExperience()
		{
			return _hero.Experience;
		}
		public int GetLevel()
		{
			return _hero.Level;
		}
		public int GetUpgradePoints()
		{
			return _hero.UpgradePoints;
		}
		public void AddItem(IItem item)
		{
			_hero.AddItem(item);
		}
        public bool RemoveItem(IItem item)
		{
			return _hero.RemoveItem(item);
		}
		public List<IItem> GetItems()
        {
            return _hero.GetItems();
        }
        public int GetItemsCount()
        {
            return _hero.GetItemsCount();
        }
		private IHero _hero { get; set; }
	}

}
