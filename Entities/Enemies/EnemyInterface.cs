using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAPZ_lab_RPG.Entities.Enemies
{
    public interface IEnemy
    {
        double Attack();
        double TakeDamage(double damageTaken);
        double Health { get; }
        double Damage { get; }
        double Armor { get; }
        string Race { get; }
    }

}