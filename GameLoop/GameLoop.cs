using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAPZ_lab_RPG.GameLoop
{
    public class GameLoop
    {
        // Enemies
        // Hero
        // Items
        public int Level { get }

        // add here fields
        public void GameLoop(){

        }  
        public void StartGameLoop()
        {
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
            // Display all stats
            Console.WriteLine("Displaying all stats...");
        }
    }
}