
namespace MAPZ_lab_RPG.Entities.Enemies
{
    public class Creature : ICreature
    {
        public Creature(double health, double damage, double armor, string name, int level){
            Health = health + 0.1 * health * level;
            Damage = damage + 0.2 * damage * level;
            Armor = armor + 0.1 * armor * level;
            Name = name;
        }
        public double Attack(){
            return Damage;
        }
        public double TakeDamage(double damageTaken){
            double damage = damageTaken / (1 + (Armor / 100));
            Health -= damage;
            return Health;
        }
        public double Health { get; set; }
        public double Damage { get; set; }
        public double Armor { get; set; }
        public string Name { get; set; }
    }
}