using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MAPZ_lab_RPG.Entities.Heroes.HeroTypes;

namespace MAPZ_lab_RPG.Entities.Heroes.Decorators
{
    public class HeroMissDecorator : HeroBaseDecorator
    {
        private readonly double _missChance;
        private readonly Random random = new Random();

        public HeroMissDecorator(IHero hero, double missChance) : base(hero)
        {
            _missChance = missChance;
        }

        public override double TakeDamage(double damageTaken)
        {
            double realDamage = damageTaken;
            if (random.NextDouble() < _missChance)
            {
                realDamage = 0;
            }
            return base.TakeDamage(realDamage);
        }
    }
}