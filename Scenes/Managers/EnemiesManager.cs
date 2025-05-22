using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using MAPZ_lab_RPG.Entities;
using MAPZ_lab_RPG.Entities.Enemies;

namespace Scenes.Managers
{
    public class EnemiesManager
    {
        private Node _enemyPlacementNode; // This should be the GridContainer
        private List<IEntity> _activeEnemies;
        private Dictionary<IEntity, Control> _enemyVisualsMap;

        private PackedScene _enemyVisualScene;
        private const string EnemyVisualScenePath = "res://Scenes/Entity.tscn";

        public event Action<IEntity> OnEnemyDefeated;
        public event Action OnAllEnemiesDefeated;
        public event Action<IEntity, Control> OnEnemyVisualClicked;

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
                _activeEnemies = new List<IEntity>();
                _enemyVisualsMap = new Dictionary<IEntity, Control>();
                return;
            }

            _activeEnemies = EnemyCreator.Instance.CreateEnemies(level);
            _enemyVisualsMap = new Dictionary<IEntity, Control>();

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
                IEntity enemy = _activeEnemies[i];
                Control enemyVisualInstance = _enemyVisualScene.Instantiate<Control>();

                if (enemyVisualInstance == null)
                {
                    GD.PrintErr($"Failed to instantiate enemy visual for {enemy.Race}.");
                    continue;
                }

                Label nameLabel = enemyVisualInstance.GetNode<Label>("VBoxContainer/CenterContainer/Label");
                ProgressBar healthBar = enemyVisualInstance.GetNode<ProgressBar>("VBoxContainer/CenterContainer3/ProgressBar");
                Label hpTextLabel = enemyVisualInstance.GetNode<Label>("VBoxContainer/CenterContainer3/ProgressBar/HPLabel");
                TextureRect icon = enemyVisualInstance.GetNode<TextureRect>("VBoxContainer/CenterContainer2/TextureRect");

                if (nameLabel == null || healthBar == null || hpTextLabel == null)
                {
                    GD.PrintErr($"Entity.tscn for {enemy.Race} is missing UI child nodes (Label, ProgressBar, HPLabel). Check paths.");
                    enemyVisualInstance.QueueFree();
                    continue;
                }

                nameLabel.Text = enemy.Race;
                healthBar.MaxValue = enemy.Health;
                healthBar.Value = enemy.Health;
                hpTextLabel.Text = $"{enemy.Health:F0} / {healthBar.MaxValue:F0}";

                string texturePath = $"res://Assets/{enemy.Race.ToLower()}.jpg";
                Texture2D texture = GD.Load<Texture2D>(texturePath);
                icon.Texture = texture;

                // Ensure Entity.tscn's root Control has Container Sizing flags set appropriately
                // (e.g., Horizontal/Vertical: Shrink Center or Expand Fill) to behave in the grid.
                enemyVisualInstance.GuiInput += (InputEvent @event) => OnEnemyClick(@event, enemy, enemyVisualInstance);

                // Add to the GridContainer. Positioning is handled by the container.
                _enemyPlacementNode.AddChild(enemyVisualInstance);
                _enemyVisualsMap.Add(enemy, enemyVisualInstance);

                GD.Print($"EnemiesManager: Added {enemy.Race} to parent container. Global position will be determined by container.");
            }
        }

        public void ApplyDamageToEnemy(IEntity enemy, double damageAmount)
        {
            if (!_activeEnemies.Contains(enemy) || !_enemyVisualsMap.ContainsKey(enemy))
            {
                GD.Print($"EnemiesManager: Attempted to damage non-existent enemy: {enemy?.Race ?? "Unknown"}.");
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

        private void HandleEnemyDefeat(IEntity defeatedEnemy)
        {
            GD.Print($"{defeatedEnemy.Race} has been defeated!");
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

        public double GetEnemyAttackDamage(IEntity attackingEnemy)
        {
            if (!_activeEnemies.Contains(attackingEnemy)) return 0;
            return attackingEnemy.Attack();
        }

        public List<IEntity> GetActiveEnemies() => new List<IEntity>(_activeEnemies);
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

        private void OnEnemyClick(InputEvent @event, IEntity enemy, Control enemyVisual)
        {
            if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                OnEnemyVisualClicked?.Invoke(enemy, enemyVisual);
            }
        }
    }
}