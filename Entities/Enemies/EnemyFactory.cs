using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAPZ_lab_RPG.Entities.Enemies
{
    public abstract class EnemyFactory
    {
        public abstract IEnemy CreateEnemy(int level);
    }
}