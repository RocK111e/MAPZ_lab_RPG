
namespace MAPZ_lab_RPG.Entities.Enemies
{
    //Tank
    public class Ork : IEntity
    {
        public Ork(int level){
            Health = 40 + 4 * level;
            Damage = 1 + 0.25 * level;
            Armor = 8 + 1 * level;
            Race = "Ork";
        }
        public double Attack(){
            System.Console.Out.WriteLine("Ork is atacking");
            return Damage;
        }
        public double TakeDamage(double damageTaken){
            Health = Health - damageTaken;
            System.Console.Out.WriteLine("Ork is being atacked");
            return Health;
        }
        public double Health { get; private set; }
        public double Damage { get; private set; }
        public double Armor { get; private set; }
        public string Race { get; private set; }
    }
    //Damager
    public class Goblin : IEntity
    {
        public Goblin(int level){
            Health = 15 + 5 * level;
            Damage = 3 + 0.3 * level;
            Armor = 3 + 0.5 * level;
            Race = "Goblin";
        }
        public double Attack(){
            System.Console.Out.WriteLine("Goblin is atacking");
            return Damage;
        }
        public double TakeDamage(double damageTaken){
            Health = Health - damageTaken;
            System.Console.Out.WriteLine("Goblin is being atacked");
            return Health;
        }
        public double Health { get; private set; }
        public double Damage { get; private set; }
        public double Armor { get; private set; }
        public string Race { get; private set; }
    }
    //Normis
    public class Troll : IEntity
    {
        public Troll(int level){
            Health = 20 + 3 * level;
            Damage = 2.5 + 0.25 * level;
            Armor = 5 + 1 * level;
            Race = "Trol";
        }
        public double Attack(){
            System.Console.Out.WriteLine("Troll is atacking");
            return Damage;
        }
        public double TakeDamage(double damageTaken){
            Health = Health - damageTaken;
            System.Console.Out.WriteLine("Troll is being atacked");
            return Health;
        }
        public double Health { get; private set; }
        public double Damage { get; private set; }
        public double Armor { get; private set; }
        public string Race { get; private set; }
    }
}