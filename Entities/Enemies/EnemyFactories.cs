using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAPZ_lab_RPG.Entities.Enemies
{
    public class OrkFactory : EnemyFactory
    {
        public override IEnemy CreateEnemy(int level)
        {
            return new Ork(level);
        }
    }

    public class GoblinFactory : EnemyFactory
    {
        public override IEnemy CreateEnemy(int level)
        {
            return new Goblin(level);
        }
    }

    public class TrolFactory : EnemyFactory
    {
        public override IEnemy CreateEnemy(int level)
        {
            return new Trol(level);
        }
    }
}