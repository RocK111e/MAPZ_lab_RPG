using System.Collections.Generic;
using MAPZ_lab_RPG.Entities.Items;
using MAPZ_lab_RPG.Entities.Enemies;
using MAPZ_lab_RPG.Entities.Creatures.Heroes.States;


namespace MAPZ_lab_RPG.Entities.Heroes
{
	public class MainHero
	{
		private static MainHero _instance;
		private IHero _hero { get; set; }
		private IHeroState _currentState;
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
		public IHeroState GetState()
        {
			return _currentState;
        }
		public void SetState(IHeroState state)
		{
			_currentState = state;
		}
		public void HeroSelect(string heroName)
		{
			_hero = EntityCreator.Instance.CreateHero(heroName);
			_currentState = new NormalState();
		}

		public List<string> GetHeroNames()
		{
			return EntityReader.Instance.ReadHeroNames();
		}

		public double Atack()
		{
			return _currentState.Attack(_hero);
		}

		public double TakeDamage(double damageTaken)
		{
			return _currentState.TakeDamage(_hero, damageTaken);
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
	}

}
