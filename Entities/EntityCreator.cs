using System;
using System.Collections.Generic;
using MAPZ_lab_RPG.Entities.Heroes;
using MAPZ_lab_RPG.Entities.Heroes.Decorators;
using MAPZ_lab_RPG.Entities.Items;

namespace MAPZ_lab_RPG.Entities.Enemies
{
	public class EntityCreator
	{
		private static readonly Dictionary<string, Func<IHero, double, IHero>> _heroDecorators =
		new()
		{
			{ "CoinMultiply", (h, v) => new HeroCoinDecorator(h, v) },
			{ "CriticalChance", (h, v) => new HeroCriticalDecorator(h, v) },
			{ "MissChance", (h, v) => new HeroMissDecorator(h, v) }
		};

		private static readonly EntityCreator _instance = new EntityCreator();

		public static EntityCreator Instance
		{
			get { return _instance; }
		}

		private EntityCreator()
		{
			enemyFactories = new List<EntityFactory>();

			EntityReader entityReader = new EntityReader();
			var enemiesData = entityReader.ReadEnemies();

			foreach (var enemyData in enemiesData)
			{
				EntityFactory enemyFactory = new EntityFactory(enemyData.Health, enemyData.Damage, enemyData.Armor, enemyData.Name);
				enemyFactories.Add(enemyFactory);
			}
		}

		private List<EntityFactory> enemyFactories;
		private readonly Random rand = new Random();

		public List<ICreature> CreateEnemies(int level)
		{
			if (level < 0)
			{
				throw new ArgumentException("Level cannot be negative", nameof(level));
			}

			List<ICreature> enemies = new List<ICreature>();

			int enemyCount = 3 + level;

			if (enemyFactories.Count == 0)
			{
				throw new InvalidOperationException("No enemy factories available. Ensure that EntityReader.ReadEnemies() returns at least one enemy.");
			}

			for (int i = 0; i < enemyCount; i++)
			{
				int choice = rand.Next(0, enemyFactories.Count);
				enemies.Add(enemyFactories[choice].CreateEntity<int>(level));
			}

			return enemies;
		}

		public IHero CreateHero(string heroName)
		{
			HeroData heroData = EntityReader.Instance.ReadHeroData(heroName);
			IHero hero = new Hero(heroData.Health, heroData.Damage, heroData.Armor, heroData.Name);

			foreach (var kvp in heroData.Additional)
			{
				if (_heroDecorators.TryGetValue(kvp.Key, out var decoratorFactory))
				{
					hero = decoratorFactory(hero, kvp.Value);
				}
				else
				{
					Console.WriteLine($"Unknown additional key: {kvp.Key}");
				}
			}

			return hero;
		}
		public List<IItem> CreateItems()
		{
			List<IItem> items = EntityReader.Instance.ReadItemsData();
			return items;
		}
	}
}
