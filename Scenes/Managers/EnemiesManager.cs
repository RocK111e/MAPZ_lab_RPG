using Godot;
using System;
using System.Collections.Generic;
using System.Linq; // Useful for methods like FirstOrDefault or ToList
using MAPZ_lab_RPG.Entities; // For IEntity
using MAPZ_lab_RPG.Entities.Enemies; // For EnemyCreator

namespace Scenes.Managers
{
    public class EnemiesManager // Made public for easier access from other scenes/scripts
    {
        private Node _enemyPlacementNode; // The parent node where enemy visuals will be added
        private List<IEntity> _activeEnemies;
        private Dictionary<IEntity, Node2D> _enemyVisualsMap; // To link IEntity logic to its Node2D visual

        private PackedScene _enemyVisualScene;
        private const string EnemyVisualScenePath = "res://Scenes/Entity.tscn";

        // Optional: Event for when an enemy is defeated
        public event Action<IEntity> OnEnemyDefeated;
        public event Action OnAllEnemiesDefeated;


        public EnemiesManager(Node enemyPlacementNode, int level)
        {
            if (enemyPlacementNode == null)
            {
                throw new ArgumentNullException(nameof(enemyPlacementNode), "Enemy placement node cannot be null.");
            }
            _enemyPlacementNode = enemyPlacementNode;

            _enemyVisualScene = GD.Load<PackedScene>(EnemyVisualScenePath);
            if (_enemyVisualScene == null)
            {
                GD.PrintErr($"EnemiesManager: Failed to load enemy visual scene at '{EnemyVisualScenePath}'.");
                // Potentially throw an exception or handle this state (e.g., no enemies can be spawned)
                _activeEnemies = new List<IEntity>();
                _enemyVisualsMap = new Dictionary<IEntity, Node2D>();
                return;
            }

            _activeEnemies = EnemyCreator.Instance.CreateEnemies(level);
            _enemyVisualsMap = new Dictionary<IEntity, Node2D>();

            SpawnAndDisplayEnemies();
        }

        private void SpawnAndDisplayEnemies()
        {
            // Basic layout - you might want something more sophisticated (HBoxContainer, VBoxContainer, or specific positions)
            float startX = 100; // Initial X position for the first enemy
            float startY = 100; // Y position for enemies
            float spacingX = 200; // Horizontal spacing between enemies

            for (int i = 0; i < _activeEnemies.Count; i++)
            {
                IEntity enemy = _activeEnemies[i];
                Node2D enemyVisualInstance = _enemyVisualScene.Instantiate<Node2D>();

                if (enemyVisualInstance == null)
                {
                    GD.PrintErr($"Failed to instantiate enemy visual for {enemy.Race}.");
                    continue; // Skip this enemy
                }

                // --- Configure Visuals ---
                Label nameLabel = enemyVisualInstance.GetNode<Label>("Label");
                ProgressBar healthBar = enemyVisualInstance.GetNode<ProgressBar>("ProgressBar");
                Label hpTextLabel = enemyVisualInstance.GetNode<Label>("ProgressBar/HPLabel"); // Path from MainHeroManager example

                if (nameLabel == null || healthBar == null || hpTextLabel == null)
                {
                    GD.PrintErr($"Entity.tscn for {enemy.Race} is missing required child nodes (Label, ProgressBar, ProgressBar/HPLabel).");
                    enemyVisualInstance.QueueFree(); // Clean up
                    // Potentially remove the problematic enemy from _activeEnemies as well
                    // or mark it as having no visual
                    continue;
                }
                
                nameLabel.Text = enemy.Race;
                healthBar.MaxValue = enemy.Health; // Assuming Health on IEntity is Max Health initially
                healthBar.Value = enemy.Health;
                hpTextLabel.Text = $"{enemy.Health:F0} / {healthBar.MaxValue:F0}";

                // --- Position and Add to Scene ---
                // This is a very basic horizontal positioning. Adjust as needed.
                enemyVisualInstance.Position = new Vector2(startX + (i * spacingX), startY);
                
                _enemyPlacementNode.AddChild(enemyVisualInstance);
                _enemyVisualsMap.Add(enemy, enemyVisualInstance);
            }
        }

        // Method to apply damage to a specific enemy and update its visuals
        public void ApplyDamageToEnemy(IEntity enemy, double damageAmount)
        {
            if (!_activeEnemies.Contains(enemy) || !_enemyVisualsMap.ContainsKey(enemy))
            {
                GD.Print($"EnemiesManager: Attempted to damage non-existent or already defeated enemy: {enemy?.Race ?? "Unknown"}.");
                return;
            }

            enemy.TakeDamage(damageAmount); // The IEntity implementation handles armor, health reduction etc.

            // Update Visuals
            Node2D visualNode = _enemyVisualsMap[enemy];
            ProgressBar healthBar = visualNode.GetNode<ProgressBar>("ProgressBar");
            Label hpTextLabel = visualNode.GetNode<Label>("ProgressBar/HPLabel");

            healthBar.Value = enemy.Health;
            hpTextLabel.Text = $"{Math.Max(0, enemy.Health):F0} / {healthBar.MaxValue:F0}"; // Ensure health doesn't go below 0 visually

            if (enemy.Health <= 0)
            {
                HandleEnemyDefeat(enemy);
            }
        }

        private void HandleEnemyDefeat(IEntity defeatedEnemy)
        {
            GD.Print($"{defeatedEnemy.Race} has been defeated!");

            if (_enemyVisualsMap.TryGetValue(defeatedEnemy, out Node2D visualNode))
            {
                visualNode.QueueFree(); // Remove visual from the scene tree
                _enemyVisualsMap.Remove(defeatedEnemy);
            }

            _activeEnemies.Remove(defeatedEnemy);
            
            OnEnemyDefeated?.Invoke(defeatedEnemy);

            if (_activeEnemies.Count == 0)
            {
                OnAllEnemiesDefeated?.Invoke();
                GD.Print("All enemies defeated!");
            }
        }

        // Method for a specific enemy to perform its attack
        // The caller (e.g., a battle turn manager) would decide which enemy attacks and who the target is.
        // This method just returns the damage value from the chosen enemy.
        public double GetEnemyAttackDamage(IEntity attackingEnemy)
        {
            if (!_activeEnemies.Contains(attackingEnemy))
            {
                GD.Print($"EnemiesManager: {attackingEnemy.Race} is not an active enemy, cannot attack.");
                return 0;
            }
            return attackingEnemy.Attack();
        }

        // Get a list of currently active enemies (returns a copy to prevent external modification)
        public List<IEntity> GetActiveEnemies()
        {
            return new List<IEntity>(_activeEnemies);
        }

        // Get a random active enemy (e.g., for player targeting)
        public IEntity GetRandomActiveEnemy()
        {
            if (_activeEnemies.Count == 0)
            {
                return null;
            }
            Random rand = new Random();
            int index = rand.Next(_activeEnemies.Count);
            return _activeEnemies[index];
        }

        // Check if there are any active enemies left
        public bool HasActiveEnemies()
        {
            return _activeEnemies.Count > 0;
        }

        // Optional: Clean up resources if the manager is no longer needed
        // (e.g., if it's part of a scene that's being freed)
        public void Cleanup()
        {
            foreach (var visualNode in _enemyVisualsMap.Values)
            {
                visualNode.QueueFree();
            }
            _enemyVisualsMap.Clear();
            _activeEnemies.Clear();
            GD.Print("EnemiesManager cleaned up.");
        }
    }
}