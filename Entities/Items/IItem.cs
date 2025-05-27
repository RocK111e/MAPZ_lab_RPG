
namespace MAPZ_lab_RPG.Entities.Items
{
    public interface IItem : IEntity
    {
        string Description { get; }
        int Price { get; }
        IItem Copy();
    }
}