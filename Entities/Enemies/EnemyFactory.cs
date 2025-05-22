
namespace MAPZ_lab_RPG.Entities.Enemies
{
    public abstract class EnemyFactory : IEntityFactory
    {
        public abstract IEntity CreateEntity<T>(T level);
    }
}