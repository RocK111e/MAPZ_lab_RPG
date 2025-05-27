using System;

namespace MAPZ_lab_RPG.Entities.Heroes.Decorators
{
    public class HeroCriticalDecorator : HeroBaseDecorator
    {
        private readonly double _criticalChance;
        private readonly Random random = new Random();

        public HeroCriticalDecorator(IHero hero, double criticalChance) : base(hero)
        {
            _criticalChance = criticalChance;
        }

        public override double Attack()
        {
            if (random.NextDouble() < _criticalChance)
            {
                return base.Attack() * 2;
            }
            return base.Attack();
        }
    }
}