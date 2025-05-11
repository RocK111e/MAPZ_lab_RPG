using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MAPZ_lab_RPG.Entities.Enemies;
using MAPZ_lab_RPG.Entities.Heroes;
using MAPZ_lab_RPG.Entities.Weapons;

namespace MAPZ_lab_RPG.GameLoop
{
    public class GameLoop
    {
        public MainHero MainHero { get; set; }
        public Enemy Enemy { get; set; }
        public Weapon Weapon { get; set; }
        public int Level { get; }

        // add here fields
        public GameLoop(){
            MainHero = MainHero.Instance;
            Enemy = new Enemy();
            Weapon = new Weapon();
            Level = 1;
        }  
        public void StartGameLoop()
        {
            AllStatsInerface();
            char Action;
            bool IsRunning = true;
            while (IsRunning) {
                FightInterface();
                Action = Console.ReadKey(true).KeyChar;
                switch (Action)
                {
                    case 'A':
                        ChooseEnemyInterface();
                        int enemyChoice = int.Parse(Console.ReadLine());
                        // Handle enemy choice
                        break;
                    case 'Q':
                        IsRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid action. Please try again.");
                        break;
                }
                AllStatsInerface();
            }
        }
        public void FightInterface()
        {
            Console.WriteLine("You are in a fight!");
            Console.WriteLine("Choose your action:");
            Console.WriteLine("A - Attack");
            Console.WriteLine("Q - Quit Game");
        }

        public void ChooseEnemyInterface()
        {
            // Display available enemies
            Console.WriteLine("Choose an enemy to fight:");
        }

        public void AllStatsInerface()
        {
            Console.WriteLine("All stats:");
            Console.WriteLine($"Hero: {MainHero.Name}, Health: {MainHero.Health}, Damage: {MainHero.Damage}, Armor: {MainHero.Armor}");
            Console.WriteLine($"Enemy: {Enemy.Race}, Health: {Enemy.Health}, Damage: {Enemy.Damage}, Armor: {Enemy.Armor}");
            Console.WriteLine($"Weapon: {Weapon.Name}, Damage: {Weapon.Damage}");
            Console.WriteLine($"Level: {Level}");
        }
    }
}