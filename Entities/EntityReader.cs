using Godot;
using System;
using System.Text.Json;
using System.Collections.Generic;

namespace MAPZ_lab_RPG.Entities
{
    public class EnemyData
    {
        public string Name { get; set; }
        public double Health { get; set; }
        public double Damage { get; set; }
        public double Armor { get; set; }
    }
    public class HeroData : EnemyData
    {
        public Dictionary<string, double> Additional { get; set; }
    }
    public class EntityReader
    {
        private const string heroJsonFilePath = "res://Assets/JSON/hero.json";
        private const string enemyJsonFilePath = "res://Assets/JSON/enemy.json";

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        public List<EnemyData> ReadEnemies()
        {
            List<EnemyData> enemies = new List<EnemyData>();
            try
            {
                if (!FileAccess.FileExists(enemyJsonFilePath))
                {
                    GD.PrintErr($"File not found: {enemyJsonFilePath}");
                    return enemies;
                }

                using var file = FileAccess.Open(enemyJsonFilePath, FileAccess.ModeFlags.Read);
                string jsonString = file.GetAsText();

                enemies = JsonSerializer.Deserialize<List<EnemyData>>(jsonString, _jsonOptions);

                if (enemies == null)
                {
                    Console.WriteLine("Deserialization returned null.");
                    return enemies;
                }

                return enemies;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
                return enemies;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return enemies;
            }
        }

        public List<string> ReadHeroNames()
        {
            List<string> heroNames = new List<string>();
            try
            {
                if (!FileAccess.FileExists(heroJsonFilePath))
                {
                    GD.PrintErr($"File not found: {heroJsonFilePath}");
                    return heroNames;
                }

                using var file = FileAccess.Open(heroJsonFilePath, FileAccess.ModeFlags.Read);
                string jsonString = file.GetAsText();

                List<HeroData> heroes = JsonSerializer.Deserialize<List<HeroData>>(jsonString, _jsonOptions);

                if (heroes == null)
                {
                    Console.WriteLine("Deserialization returned null.");
                    return heroNames;
                }
                foreach (var hero in heroes)
                {
                    heroNames.Add(hero.Name);
                }

                return heroNames;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
                return heroNames;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return heroNames;
            }
        }

        public HeroData GetHeroData(string heroName)
        {
            try
            {
                if (!FileAccess.FileExists(heroJsonFilePath))
                {
                    GD.PrintErr($"File not found: {heroJsonFilePath}");
                    return null;
                }

                using var file = FileAccess.Open(heroJsonFilePath, FileAccess.ModeFlags.Read);
                string jsonString = file.GetAsText();

                List<HeroData> heroes = JsonSerializer.Deserialize<List<HeroData>>(jsonString, _jsonOptions);

                if (heroes == null)
                {
                    Console.WriteLine("Deserialization returned null.");
                    return null;
                }
                foreach (var hero in heroes)
                {

                    if (hero.Name == heroName)
                    {
                        return hero;
                    }
                }

                return null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return null;
            }
        }
    }
}