using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MAPZ_lab_RPG.Entities;

namespace MAPZ_lab_RPG.Entities.Enemies
{
    public abstract class EnemyFactory : IEntityFactory
    {
        public abstract IEntity CreateEntity<T>(T level);
    }
}