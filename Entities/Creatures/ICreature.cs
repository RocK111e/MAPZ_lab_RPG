
namespace MAPZ_lab_RPG.Entities
{
	public interface ICreature : IEntity
	{
		double Attack();
		double TakeDamage(double damageTaken);
	}
}
