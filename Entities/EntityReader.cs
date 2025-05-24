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
    public class EntityReader
    {
        public List<EnemyData> ReadEnemies()
        {
            List<EnemyData> enemies = new List<EnemyData>();
            try
            {
                string jsonFilePath = "res://Assets/JSON/enemy.json";

                if (!FileAccess.FileExists(jsonFilePath))
                {
                    GD.PrintErr($"File not found: {jsonFilePath}");
                    return enemies;
                }

                using var file = FileAccess.Open(jsonFilePath, FileAccess.ModeFlags.Read);
                string jsonString = file.GetAsText();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                enemies = JsonSerializer.Deserialize<List<EnemyData>>(jsonString, options);

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
                string jsonFilePath = "res://Assets/JSON/hero.json";

                if (!FileAccess.FileExists(jsonFilePath))
                {
                    GD.PrintErr($"File not found: {jsonFilePath}");
                    return heroNames;
                }

                using var file = FileAccess.Open(jsonFilePath, FileAccess.ModeFlags.Read);
                string jsonString = file.GetAsText();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                List<EnemyData> heroes = JsonSerializer.Deserialize<List<EnemyData>>(jsonString, options);

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

        public EnemyData GetHeroData(string heroName)
        {
            try
            {
                string jsonFilePath = "res://Assets/JSON/hero.json";

                if (!FileAccess.FileExists(jsonFilePath))
                {
                    GD.PrintErr($"File not found: {jsonFilePath}");
                    return null;
                }

                using var file = FileAccess.Open(jsonFilePath, FileAccess.ModeFlags.Read);
                string jsonString = file.GetAsText();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                List<EnemyData> heroes = JsonSerializer.Deserialize<List<EnemyData>>(jsonString, options);

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