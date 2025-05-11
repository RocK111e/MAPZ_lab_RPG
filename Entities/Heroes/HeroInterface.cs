using System.Collections.Generic;
using MAPZ_lab_RPG.Entities.Items;
namespace MAPZ_lab_RPG.Entities.Heroes

interface IHero
{
    public string Name { get; private set; }
    public float Atack();
    public float GetDamage(float Damage);
    public float Heal(float HealAmount);
    public void LevelUp();
    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    public float Damage { get; private set; }
    public float Armor { get; private set; }

    public int Coins { get; private set; }
    public int AddCoins(int coins)
    {
        Coins += coins;
        return Coins;
    }
    public int Experience { get; private set; }
    public void AddExperience(int experience)
    {
        Experience += experience;
        if (Experience >= 100)
        {
            LevelUp();
            Experience -= 100;
        }
    }
    public int Level { get; private set; }
    public int LevelPoints{ get; private set; }
    public void Upgrade(string atribute)

    public List<Items> Inventory { get; private set; }
    public void AddItem(Items item){
        Inventory.Add(item);
    }
}