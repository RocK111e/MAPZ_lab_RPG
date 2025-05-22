

namespace MAPZ_lab_RPG.Entities
{
    public interface IEntityFactory
    {
        public IEntity CreateEntity<T>(T param);
    }
}