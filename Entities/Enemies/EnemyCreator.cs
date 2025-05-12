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
            TrollFactory = new TrollFactory();
        }

        private EnemyFactory OrkFactory { get; }
        private EnemyFactory GoblinFactory { get; }
        private EnemyFactory TrollFactory { get; }

        public List<IEnemy> CreateEnemies(int level)
        {
            if (level < 0){
                throw new ArgumentException("Level cannot be negative", nameof(level));
            }
            List<IEnemy> enemies = new List<IEnemy>();

            List<int> enemyList = CreateEnemyList(level);
            foreach (int choice in enemyList)
            {
                switch (choice)
                {
                    case (1):
                        enemies.Add(OrkFactory.CreateEnemy(level));
                        break;
                    case (2):
                        enemies.Add(GoblinFactory.CreateEnemy(level));
                        break;
                    case (3):
                        enemies.Add(TrollFactory.CreateEnemy(level));
                        break;
                    default:
                        System.Console.Out.WriteLine("Error occurred: Invalid enemy choice");
                        break;
                }
                
            }

            return enemies;
        }
        public List<int> CreateEnemyList(int level){
            List<int> enemyList = new List<int>();
            Random rand = new Random();
            int enemyCount = 3 + level;
            for (int i = 0; i < enemyCount; i++)
            {
                int choice = rand.Next(1,4);
                enemyList.Add(choice);
            }
            return enemyList;
        }
    }
}