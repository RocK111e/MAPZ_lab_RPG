using MAPZ_lab_RPG.Entities.Heroes;

namespace MAPZ_lab_RPG.Entities.Creatures.Heroes.States
{
    public class NormalState : IHeroState
    {
        public double Attack(IHero hero) => hero.Damage;
        public double TakeDamage(IHero hero, double damageTaken) => damageTaken;
        public string GetStateName() => "Normal";
    }
}