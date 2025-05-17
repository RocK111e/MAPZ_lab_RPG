using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            enemyFactories = new List<EnemyFactory>()
            {
                new OrkFactory(),
                new GoblinFactory(),
                new TrollFactory()
            };
        }

        private List<EnemyFactory> enemyFactories;

        public List<IEntity> CreateEnemies(int level)
        {
            if (level < 0){
                throw new ArgumentException("Level cannot be negative", nameof(level));
            }

            List<IEntity> enemies = new List<IEntity>();

            Random rand = new Random();
            int enemyCount = 3 + level;

            for (int i = 0; i < enemyCount; i++)
            {
                int choice = rand.Next(0, enemyFactories.Count);
                enemies.Add(enemyFactories[choice].CreateEntity<int>(level));
            }

            return enemies;
        }
    }
}