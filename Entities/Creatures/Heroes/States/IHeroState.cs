using MAPZ_lab_RPG.Entities.Heroes;

namespace MAPZ_lab_RPG.Entities.Creatures.Heroes.States
{
    public interface IHeroState
    {
        double Attack(IHero hero);
        double TakeDamage(IHero hero, double damageTaken);
        string GetStateName();
    }
}