
namespace MAPZ_lab_RPG.Entities
{
	public interface IEntity
	{
		double Health { get; set; }
		double Damage { get; set; }
		double Armor { get; set; }
		string Name { get; set; }
	}
}
