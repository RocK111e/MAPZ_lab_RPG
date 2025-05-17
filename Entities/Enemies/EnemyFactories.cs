using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAPZ_lab_RPG.Entities.Enemies
{
    public class OrkFactory : EnemyFactory
    {
        public override IEntity CreateEntity<T>(T level)
        {
            if (level is int intLevel)
            {
                return new Ork(intLevel);
            }

            throw new ArgumentException("Level must be an int.");
        }
    }

    public class GoblinFactory : EnemyFactory
    {
        public override IEntity CreateEntity<T>(T level)
        {
            if (level is int intLevel)
            {
                return new Goblin(intLevel);
            }

            throw new ArgumentException("Level must be an int.");
        }
    }

    public class TrollFactory : EnemyFactory
    {
        public override IEntity CreateEntity<T>(T level)
        {
            if (level is int intLevel)
            {
                return new Troll(intLevel);
            }

            throw new ArgumentException("Level must be an int.");
        }
    }
}