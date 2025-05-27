
namespace MAPZ_lab_RPG.Entities
{
    public interface IEntityFactory
    {
        public ICreature CreateEntity<T>(T param);
    }
}