

namespace MAPZ_lab_RPG.Entities
{
    public interface IEntity
    {
        double Attack();
        double TakeDamage(double damageTaken);
        double Health { get; }
        double Damage { get; }
        double Armor { get; }
        string Name { get; }
    }
}