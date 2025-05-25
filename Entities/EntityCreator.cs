using System;
using System.Collections.Generic;

namespace MAPZ_lab_RPG.Entities.Enemies
{
	public class EntityCreator
	{
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


		public List<IEntity> CreateEnemies(int level)
		{
			if (level < 0)
			{
				throw new ArgumentException("Level cannot be negative", nameof(level));
			}

			List<IEntity> enemies = new List<IEntity>();

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
	}
}
