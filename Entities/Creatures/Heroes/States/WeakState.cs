using MAPZ_lab_RPG.Entities.Heroes;

namespace MAPZ_lab_RPG.Entities.Creatures.Heroes.States
{
    public class WeakState : IHeroState
    {
        public double Attack(IHero hero) => hero.Damage * 0.8;
        public double TakeDamage(IHero hero, double damageTaken) => damageTaken * 1.2;
        public string GetStateName() => "Weak";
    }
}