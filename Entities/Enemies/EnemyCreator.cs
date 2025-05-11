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
            OrkFactory = new OrkFactory();
            GoblinFactory = new GoblinFactory();
            TrolFactory = new TrolFactory();
        }

        private EnemyFactory OrkFactory { get; }
        private EnemyFactory GoblinFactory { get; }
        private EnemyFactory TrolFactory { get; }

        public List<IEnemy> CreateEnemies(int level)
        {
            if (level < 0)
                throw new ArgumentException("Level cannot be negative", nameof(level));

            List<IEnemy> enemies = new List<IEnemy>
            {
                OrkFactory.CreateEnemy(level),
                GoblinFactory.CreateEnemy(level),
                TrolFactory.CreateEnemy(level)
            };

            return enemies;
        }
    }
}