using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAPZ_lab_RPG.Entities
{
    public interface IEntityFactory
    {
        public IEntity CreateEntity<T>(T param);
    }
}