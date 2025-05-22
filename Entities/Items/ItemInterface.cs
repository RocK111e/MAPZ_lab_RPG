
namespace MAPZ_lab_RPG.Entities.Items
{
    public interface IItem
    {
        string Name { get; }
        string Description { get; }
        double Health { get; }
        double Damage { get; }
        double Armor { get; }
        int Price { get; }
        IItem Copy();
    }
}