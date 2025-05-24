
namespace MAPZ_lab_RPG.Entities.Enemies
{
    public class EntityFactory : IEntityFactory
    {
        public EntityFactory(double health, double damage, double armor, string name)
        {
            Health = health;
            Damage = damage;
            Armor = armor;
            Name = name;
        }
        public IEntity CreateEntity<T>(T param)
        {
            if (param is int intLevel){
                return new Entity(
                    Health,
                    Damage,
                    Armor,
                    Name,
                    intLevel
                );
            }
            return null;
        }
        public double Health { get; private set; }
        public double Damage { get; private set; }
        public double Armor { get; private set; }
        public string Name { get; private set; }
    }
}