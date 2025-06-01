using MAPZ_lab_RPG.Entities.Heroes;

namespace MAPZ_lab_RPG.Entities.Creatures.Heroes.States
{
    public class StrongState : IHeroState
    {
        public double Attack(IHero hero) => hero.Damage * 1.2;
        public double TakeDamage(IHero hero, double damageTaken) => damageTaken * 0.8;
        public string GetStateName() => "Strong";
    }
}