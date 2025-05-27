using Godot;
using System;
using System.Collections.Generic;
using MAPZ_lab_RPG.Entities;
using MAPZ_lab_RPG.Entities.Enemies;

namespace Scenes.Managers
{
    public class EnemiesManager
    {
        private Node _enemyPlacementNode;
        private List<ICreature> _activeEnemies;
        private Dictionary<ICreature, Control> _enemyVisualsMap;

        private PackedScene _enemyVisualScene;
        private const string EnemyVisualScenePath = "res://Scenes/Entity.tscn";

        public event Action<ICreature> OnEnemyDefeated;
        public event Action OnAllEnemiesDefeated;
        public event Action<ICreature, Control> OnEnemyVisualClicked;

        public EnemiesManager(Node enemyPlacementNode, int level)
        {
            if (enemyPlacementNode == null)
            {
                throw new ArgumentNullException(nameof(enemyPlacementNode), "Enemy placement node cannot be null.");
            }
            if (!(enemyPlacementNode is GridContainer))
            {
                GD.PrintRich($"[color=yellow]EnemiesManager Warning: enemyPlacementNode is a {enemyPlacementNode.GetType().Name}, not a GridContainer. Layout will be manual AddChild without grid benefits.[/color]");
            }
            _enemyPlacementNode = enemyPlacementNode;

            _enemyVisualScene = GD.Load<PackedScene>(EnemyVisualScenePath);
            if (_enemyVisualScene == null)
            {
                GD.PrintErr($"EnemiesManager: Failed to load enemy visual scene at '{EnemyVisualScenePath}'.");
                _activeEnemies = new List<ICreature>();
                _enemyVisualsMap = new Dictionary<ICreature, Control>();
                return;
            }

            _activeEnemies = EntityCreator.Instance.CreateEnemies(level);
            _enemyVisualsMap = new Dictionary<ICreature, Control>();

            SpawnAndDisplayEnemies();
        }

        private void SpawnAndDisplayEnemies()
        {
            if (_activeEnemies.Count == 0)
            {
                GD.Print("EnemiesManager: No enemies to spawn.");
                return;
            }

            GD.Print($"EnemiesManager: Spawning {_activeEnemies.Count} enemies into node: {_enemyPlacementNode.GetPath()} (should be GridContainer)");

            for (int i = 0; i < _activeEnemies.Count; i++)
            {
                ICreature enemy = _activeEnemies[i];
                Control enemyVisualInstance = _enemyVisualScene.Instantiate<Control>();

                if (enemyVisualInstance == null)
                {
                    GD.PrintErr($"Failed to instantiate enemy visual for {enemy.Name}.");
                    continue;
                }

                Label nameLabel = enemyVisualInstance.GetNode<Label>("VBoxContainer/CenterContainer/Label");
                ProgressBar healthBar = enemyVisualInstance.GetNode<ProgressBar>("VBoxContainer/CenterContainer3/ProgressBar");
                Label hpTextLabel = enemyVisualInstance.GetNode<Label>("VBoxContainer/CenterContainer3/ProgressBar/HPLabel");
                TextureRect icon = enemyVisualInstance.GetNode<TextureRect>("VBoxContainer/CenterContainer2/TextureRect");

                if (nameLabel == null || healthBar == null || hpTextLabel == null)
                {
                    GD.PrintErr($"Entity.tscn for {enemy.Name} is missing UI child nodes (Label, ProgressBar, HPLabel). Check paths.");
                    enemyVisualInstance.QueueFree();
                    continue;
                }

                nameLabel.Text = enemy.Name;
                healthBar.MaxValue = enemy.Health;
                healthBar.Value = enemy.Health;
                hpTextLabel.Text = $"{enemy.Health:F0} / {healthBar.MaxValue:F0}";

                string texturePath = $"res://Assets/{enemy.Name.ToLower()}.jpg";
                Texture2D texture = GD.Load<Texture2D>(texturePath);
                icon.Texture = texture;

                enemyVisualInstance.GuiInput += (InputEvent @event) => OnEnemyClick(@event, enemy, enemyVisualInstance);

                _enemyPlacementNode.AddChild(enemyVisualInstance);
                _enemyVisualsMap.Add(enemy, enemyVisualInstance);

                GD.Print($"EnemiesManager: Added {enemy.Name} to parent container. Global position will be determined by container.");
            }
        }

        public void ApplyDamageToEnemy(ICreature enemy, double damageAmount)
        {
            if (!_activeEnemies.Contains(enemy) || !_enemyVisualsMap.ContainsKey(enemy))
            {
                GD.Print($"EnemiesManager: Attempted to damage non-existent enemy: {enemy?.Name ?? "Unknown"}.");
                return;
            }
            enemy.TakeDamage(damageAmount);

            if (_enemyVisualsMap.TryGetValue(enemy, out Control visualNode))
            {
                ProgressBar healthBar = visualNode.GetNode<ProgressBar>("VBoxContainer/CenterContainer3/ProgressBar");
                Label hpTextLabel = visualNode.GetNode<Label>("VBoxContainer/CenterContainer3/ProgressBar/HPLabel");
                healthBar.Value = enemy.Health;
                hpTextLabel.Text = $"{Math.Max(0, enemy.Health):F0} / {healthBar.MaxValue:F0}";
            }

            if (enemy.Health <= 0)
            {
                HandleEnemyDefeat(enemy);
            }
        }

        private void HandleEnemyDefeat(ICreature defeatedEnemy)
        {
            GD.Print($"{defeatedEnemy.Name} has been defeated!");
            if (_enemyVisualsMap.TryGetValue(defeatedEnemy, out Control visualNode))
            {
                visualNode.QueueFree();
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

        public double GetEnemyAttackDamage(ICreature attackingEnemy)
        {
            if (!_activeEnemies.Contains(attackingEnemy)) return 0;
            return attackingEnemy.Attack();
        }

        public List<ICreature> GetActiveEnemies() => new List<ICreature>(_activeEnemies);
        public bool HasActiveEnemies() => _activeEnemies.Count > 0;

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

        private void OnEnemyClick(InputEvent @event, ICreature enemy, Control enemyVisual)
        {
            if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                OnEnemyVisualClicked?.Invoke(enemy, enemyVisual);
            }
        }
    }
}