using System;
using System.Collections.Generic;

namespace MAPZ_lab_RPG.Entities.Enemies
{
	public class EnemyCreator
	{
		private static readonly EnemyCreator _instance = new EnemyCreator();

		public static EnemyCreator Instance
		{
			get { return _instance; }
		}

		private EnemyCreator()
		{
			entityFactories = new List<EntityFactory>();

			EntityReader entityReader = new EntityReader();
			List<Entity> enemies = entityReader.ReadEnemies();
			Console.Out.WriteLine("\n\n\n enemies: ", enemies, "\n\n\n");

			foreach (Entity enemy in enemies)
			{
				EntityFactory entityFactory = new EntityFactory(enemy.Health, enemy.Damage, enemy.Armor, enemy.Name);
				entityFactories.Add(entityFactory);
			}
		}

		private List<EntityFactory> entityFactories;
		private readonly Random rand = new Random();


		public List<IEntity> CreateEnemies(int level)
		{
			if (level < 0)
			{
				throw new ArgumentException("Level cannot be negative", nameof(level));
			}

			List<IEntity> enemies = new List<IEntity>();

			int enemyCount = 3 + level;

			if (entityFactories.Count == 0)
			{
				throw new InvalidOperationException("No enemy factories available. Ensure that EntityReader.ReadEnemies() returns at least one enemy.");
			}

			for (int i = 0; i < enemyCount; i++)
			{
				int choice = rand.Next(0, entityFactories.Count);
				enemies.Add(entityFactories[choice].CreateEntity<int>(level));
			}

			return enemies;
		}
	}
}
