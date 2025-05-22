using Godot;
using System;
using System.Collections.Generic;
using System.Linq; // Useful for methods like FirstOrDefault or ToList
using MAPZ_lab_RPG.Entities; // For IEntity
using MAPZ_lab_RPG.Entities.Enemies; // For EnemyCreator

namespace Scenes.Managers
{
    public class EnemiesManager
    {
        private Node _enemyPlacementNode;
        private List<IEntity> _activeEnemies;
        private Dictionary<IEntity, Control> _enemyVisualsMap; // Changed Node2D to Control to match Entity.tscn root

        private PackedScene _enemyVisualScene;
        private const string EnemyVisualScenePath = "res://Scenes/Entity.tscn"; // Path to your entity scene

        // Event for when an enemy is defeated
        public event Action<IEntity> OnEnemyDefeated;
        public event Action OnAllEnemiesDefeated;

        // Event for when an enemy visual is clicked
        public event Action<IEntity, Control> OnEnemyVisualClicked; // Passes the logical enemy and its Control visual

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
            float startX = 100; // Initial X position
            float startY = 100; // Y position
            float spacingX = 250; // Horizontal spacing, adjust based on your Entity.tscn size

            for (int i = 0; i < _activeEnemies.Count; i++)
            {
                IEntity enemy = _activeEnemies[i];
                // Instantiate Entity.tscn. Its root is Control.
                Control enemyVisualInstance = _enemyVisualScene.Instantiate<Control>();

                if (enemyVisualInstance == null)
                {
                    GD.PrintErr($"Failed to instantiate enemy visual for {enemy.Race}.");
                    continue;
                }

                // --- Configure Visuals (Paths based on your Entity.tscn screenshot) ---
                Label nameLabel = enemyVisualInstance.GetNode<Label>("VBoxContainer/CenterContainer/Label");
                ProgressBar healthBar = enemyVisualInstance.GetNode<ProgressBar>("VBoxContainer/CenterContainer3/ProgressBar");
                Label hpTextLabel = enemyVisualInstance.GetNode<Label>("VBoxContainer/CenterContainer3/ProgressBar/HPLabel");
                // TextureRect for sprite (if you load it dynamically)
                // TextureRect sprite = enemyVisualInstance.GetNode<TextureRect>("VBoxContainer/CenterContainer2/TextureRect");

                if (nameLabel == null || healthBar == null || hpTextLabel == null)
                {
                    GD.PrintErr($"Entity.tscn for {enemy.Race} is missing required child nodes (Label, ProgressBar, HPLabel). Check paths.");
                    enemyVisualInstance.QueueFree();
                    continue;
                }

                nameLabel.Text = enemy.Race;
                healthBar.MaxValue = enemy.Health;
                healthBar.Value = enemy.Health;
                hpTextLabel.Text = $"{enemy.Health:F0} / {healthBar.MaxValue:F0}";

                // --- Setup Clickable Area ---
                // The Area2D is a direct child of the root "Control" node in Entity.tscn
                Area2D clickableArea = enemyVisualInstance.GetNode<Area2D>("Area2D");
                if (clickableArea != null)
                {
                    // Connect the input_event signal.
                    clickableArea.InputEvent += (viewport, eventArgs, shapeIdx) =>
                        HandleEnemyInput(viewport, eventArgs, shapeIdx, enemy, enemyVisualInstance);
                    GD.Print($"Connected input event for {enemy.Race}'s Area2D.");
                }
                else
                {
                    GD.PrintErr($"EnemiesManager: 'Area2D' node not found in Entity.tscn for {enemy.Race}. This enemy will not be clickable.");
                }


                // --- Position and Add to Scene ---
                // Since the root is a Control, setting Position works.
                // If _enemyPlacementNode is a container, it might override this.
                // For now, assume _enemyPlacementNode allows manual positioning of its children.
                enemyVisualInstance.Position = new Vector2(startX + (i * spacingX), startY);

                _enemyPlacementNode.AddChild(enemyVisualInstance);
                _enemyVisualsMap.Add(enemy, enemyVisualInstance);
            }
        }

        private void HandleEnemyInput(Node viewport, InputEvent eventArgs, long shapeIdx, IEntity enemy, Control visual)
        {
            if (eventArgs is InputEventMouseButton mouseButtonEvent)
            {
                if (mouseButtonEvent.ButtonIndex == MouseButton.Left && mouseButtonEvent.Pressed)
                {
                    GD.Print($"EnemiesManager: Clicked on enemy visual for {enemy.Race}");
                    OnEnemyVisualClicked?.Invoke(enemy, visual); // Raise the event
                }
            }
        }

        public void ApplyDamageToEnemy(IEntity enemy, double damageAmount)
        {
            if (!_activeEnemies.Contains(enemy) || !_enemyVisualsMap.ContainsKey(enemy))
            {
                GD.Print($"EnemiesManager: Attempted to damage non-existent or already defeated enemy: {enemy?.Race ?? "Unknown"}.");
                return;
            }

            enemy.TakeDamage(damageAmount);

            Control visualNode = _enemyVisualsMap[enemy];
            ProgressBar healthBar = visualNode.GetNode<ProgressBar>("VBoxContainer/CenterContainer3/ProgressBar");
            Label hpTextLabel = visualNode.GetNode<Label>("VBoxContainer/CenterContainer3/ProgressBar/HPLabel");

            healthBar.Value = enemy.Health;
            hpTextLabel.Text = $"{Math.Max(0, enemy.Health):F0} / {healthBar.MaxValue:F0}";

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
            if (!_activeEnemies.Contains(attackingEnemy))
            {
                GD.Print($"EnemiesManager: {attackingEnemy.Race} is not an active enemy, cannot attack.");
                return 0;
            }
            return attackingEnemy.Attack();
        }

        public List<IEntity> GetActiveEnemies()
        {
            return new List<IEntity>(_activeEnemies);
        }

        public IEntity GetRandomActiveEnemy()
        {
            if (_activeEnemies.Count == 0) return null;
            Random rand = new Random();
            return _activeEnemies[rand.Next(_activeEnemies.Count)];
        }

        public bool HasActiveEnemies()
        {
            return _activeEnemies.Count > 0;
        }

        public void Cleanup()
        {
            foreach (var visualNode in _enemyVisualsMap.Values)
            {
                // Before queuing free, good practice to ensure signals are disconnected
                // if the handler might still exist or cause issues.
                // However, for simple lambda captures like this, it's often okay.
                Area2D clickableArea = visualNode.GetNodeOrNull<Area2D>("Area2D");
                if (clickableArea != null)
                {
                    // Manually disconnect if you had stored Callables, not strictly necessary for lambdas here
                    // when the object holding the lambda (EnemiesManager) is also being disposed or out of scope.
                }
                visualNode.QueueFree();
            }
            _enemyVisualsMap.Clear();
            _activeEnemies.Clear();
            GD.Print("EnemiesManager cleaned up.");
        }
    }
}