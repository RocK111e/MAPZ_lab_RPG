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
        private readonly string _jsonPath = "res://Assets/JSON/";
        public List<Enemies.Entity> ReadEnemies()
        {
            List<Enemies.Entity> enemies = new List<Enemies.Entity>();

            try
            {
                string jsonFilePath = "res://Assets/JSON/enemy.json";
                Console.WriteLine($"Looking for file at: {jsonFilePath}");

                if (!FileAccess.FileExists(jsonFilePath))
                {
                    GD.PrintErr($"File not found: {jsonFilePath}");
                    return enemies;
                }

                using var file = FileAccess.Open(jsonFilePath, FileAccess.ModeFlags.Read);
                string jsonString = file.GetAsText();
                Console.WriteLine("JSON content read from file:");
                Console.WriteLine(jsonString);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                List<EnemyData> enemiesData = JsonSerializer.Deserialize<List<EnemyData>>(jsonString, options);

                if (enemiesData == null)
                {
                    Console.WriteLine("Deserialization returned null.");
                    return enemies;
                }

                foreach (var enemyData in enemiesData)
                {
                    var enemy = new Enemies.Entity(
                        health: enemyData.Health,
                        damage: enemyData.Damage,
                        armor: enemyData.Armor,
                        name: enemyData.Name,
                        level: 1
                    );
                    enemies.Add(enemy);
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
    }
}