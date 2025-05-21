using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using MAPZ_lab_RPG.Entities; // Required for IEntity and IEntityFactory

namespace Scenes.Managers
{
    class EnemiesManager
    {
        private Node _enemyContainerNode;    // Parent Node in the scene tree where enemy visuals will be added
        private PackedScene _enemyVisualPackedScene; // Pre-loaded PackedScene for an individual enemy
        private IEntityFactory _entityFactory; // Factory to create IEntity instances

        // Helper class to bundle an IEntity with its Godot Node and UI elements
        private class ManagedEnemy
        {
            public IEntity Entity { get; }
            public Node2D NodeInstance { get; } // Root node of the instantiated enemy scene
            public ProgressBar HealthBar { get; }
            public double MaxHealth { get; }

            public ManagedEnemy(IEntity entity, Node2D nodeInstance)
            {
                Entity = entity;
                NodeInstance = nodeInstance;
                MaxHealth = Entity.Health;

                HealthBar = NodeInstance.GetNodeOrNull<ProgressBar>("ProgressBar"); // Adjust path if needed
                if (HealthBar == null)
                {
                    GD.PushWarning($"Enemy instance '{NodeInstance.Name}' (scene: '{NodeInstance.SceneFilePath}') " +
                                   "is missing a ProgressBar node named 'ProgressBar'. Health UI will not function.");
                }
                else
                {
                    HealthBar.MaxValue = MaxHealth > 0 ? MaxHealth : 1;
                    HealthBar.Value = Entity.Health;
                }
            }

            public void UpdateHealthUI()
            {
                if (HealthBar != null)
                {
                    HealthBar.Value = Entity.Health;
                }
            }

            public void CleanUp()
            {
                NodeInstance.QueueFree();
            }
        }

        private Dictionary<IEntity, ManagedEnemy> _activeEnemies;

        /// <summary>
        /// Initializes a new instance of the EnemiesManager.
        /// </summary>
        /// <param name="enemyContainerNode">The parent Node where enemy scenes will be instanced as children.</param>
        /// <param name="enemyScenePath">The resource path to the PackedScene for an enemy (e.g., "res://Scenes/Entity.tscn").</param>
        /// <param name="entityFactory">The factory used to create IEntity data objects.</param>
        public EnemiesManager(Node enemyContainerNode, string enemyScenePath, IEntityFactory entityFactory)
        {
            _enemyContainerNode = enemyContainerNode ?? throw new ArgumentNullException(nameof(enemyContainerNode));
            _entityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory));
            _activeEnemies = new Dictionary<IEntity, ManagedEnemy>();

            if (string.IsNullOrEmpty(enemyScenePath))
            {
                throw new ArgumentException("Enemy scene path cannot be null or empty.", nameof(enemyScenePath));
            }
            _enemyVisualPackedScene = GD.Load<PackedScene>(enemyScenePath);
            if (_enemyVisualPackedScene == null)
            {
                throw new ArgumentException($"Failed to load PackedScene from path: {enemyScenePath}", nameof(enemyScenePath));
            }
        }

        /// <summary>
        /// Spawns a single enemy.
        /// </summary>
        /// <typeparam name="TParam">Type of parameter for entity creation.</typeparam>
        /// <param name="creationParam">Parameter for the entity factory.</param>
        /// <param name="position">Global position to spawn the enemy.</param>
        /// <returns>The created IEntity, or null on failure.</returns>
        public IEntity SpawnEnemy<TParam>(TParam creationParam, Vector2 position)
        {
            IEntity enemyEntity = _entityFactory.CreateEntity(creationParam);
            if (enemyEntity == null)
            {
                GD.PushError($"EntityFactory failed to create entity with param: {creationParam}");
                return null;
            }

            Node2D enemyNode2D = _enemyVisualPackedScene.Instantiate<Node2D>();
            if (enemyNode2D == null)
            {
                GD.PushError($"Failed to instantiate enemy scene ('{_enemyVisualPackedScene.ResourcePath}') or its root is not Node2D.");
                // Potentially clean up enemyEntity if it has disposable resources, though IEntity doesn't specify
                return null;
            }
            
            enemyNode2D.GlobalPosition = position;
            _enemyContainerNode.AddChild(enemyNode2D);

            ManagedEnemy managedEnemy = new ManagedEnemy(enemyEntity, enemyNode2D);
            _activeEnemies.Add(enemyEntity, managedEnemy);
            
            GD.Print($"Spawned enemy '{enemyEntity.Race}' (HP: {enemyEntity.Health}/{managedEnemy.MaxHealth}) at {position}.");
            return enemyEntity;
        }

        /// <summary>
        /// Spawns multiple enemies based on provided parameters and positions.
        /// </summary>
        /// <typeparam name="TParam">The type of parameter the IEntityFactory's CreateEntity method expects.</typeparam>
        /// <param name="creationParams">A list of parameters, one for each enemy to be created by the factory.</param>
        /// <param name="positions">A list of Vector2 global positions, corresponding to each enemy.</param>
        /// <returns>A list of successfully spawned IEntity objects.</returns>
        public List<IEntity> SpawnMultipleEnemies<TParam>(List<TParam> creationParams, List<Vector2> positions)
        {
            if (creationParams == null) throw new ArgumentNullException(nameof(creationParams));
            if (positions == null) throw new ArgumentNullException(nameof(positions));
            if (creationParams.Count != positions.Count)
            {
                GD.PushError("EnemiesManager: Mismatch between number of enemy creation parameters and positions. Cannot spawn.");
                return new List<IEntity>(); // Return empty list on critical error
            }

            List<IEntity> spawnedEntities = new List<IEntity>();
            for (int i = 0; i < creationParams.Count; i++)
            {
                TParam currentParam = creationParams[i];
                Vector2 currentPosition = positions[i];

                IEntity enemyEntity = _entityFactory.CreateEntity(currentParam);
                if (enemyEntity == null)
                {
                    GD.PushWarning($"EnemiesManager: EntityFactory failed to create entity for param: {currentParam} at index {i}. Skipping this enemy.");
                    continue; // Skip to the next enemy
                }

                // Instantiate the scene using the preloaded PackedScene
                Node2D enemyNode2D = _enemyVisualPackedScene.Instantiate<Node2D>();
                if (enemyNode2D == null)
                {
                    GD.PushWarning($"EnemiesManager: Failed to instantiate enemy scene ('{_enemyVisualPackedScene.ResourcePath}') or its root is not Node2D, for enemy with param {currentParam} at index {i}. Skipping this enemy.");
                    // Potentially clean up enemyEntity if it has disposable resources
                    continue; // Skip to the next enemy
                }
                
                enemyNode2D.GlobalPosition = currentPosition;
                _enemyContainerNode.AddChild(enemyNode2D);

                ManagedEnemy managedEnemy = new ManagedEnemy(enemyEntity, enemyNode2D);
                _activeEnemies.Add(enemyEntity, managedEnemy);
                spawnedEntities.Add(enemyEntity);

                GD.Print($"EnemiesManager: Spawned enemy '{enemyEntity.Race}' (HP: {enemyEntity.Health}/{managedEnemy.MaxHealth}) at {currentPosition}.");
            }
            return spawnedEntities;
        }


        public void DamageEnemy(IEntity enemy, double damageAmount)
        {
            if (enemy == null)
            {
                GD.PushWarning("EnemiesManager: Attempted to damage a null enemy reference.");
                return;
            }

            if (!_activeEnemies.TryGetValue(enemy, out ManagedEnemy managedEnemy))
            {
                GD.PushWarning($"EnemiesManager: Attempted to damage enemy '{enemy.Race}' (Hash: {enemy.GetHashCode()}) not currently managed or already removed.");
                return;
            }

            double healthBeforeDamage = enemy.Health;
            enemy.TakeDamage(damageAmount);
            managedEnemy.UpdateHealthUI();

            GD.Print($"EnemiesManager: Enemy '{enemy.Race}' (HP: {healthBeforeDamage} -> {enemy.Health}) took {damageAmount} damage input.");

            if (enemy.Health <= 0)
            {
                HandleEnemyDeath(enemy, managedEnemy);
            }
        }

        private void HandleEnemyDeath(IEntity enemy, ManagedEnemy managedEnemy)
        {
            GD.Print($"EnemiesManager: Enemy '{enemy.Race}' has died.");
            // Add any on-death logic here (e.g., drop loot, grant experience)
            // Example: GetTree().Root.GetNode<LootManager>("LootManager").SpawnLoot(enemy.LootTable, managedEnemy.NodeInstance.GlobalPosition);

            managedEnemy.CleanUp();
            _activeEnemies.Remove(enemy);
        }

        public bool RemoveEnemy(IEntity enemy)
        {
            if (enemy != null && _activeEnemies.TryGetValue(enemy, out ManagedEnemy managedEnemy))
            {
                GD.Print($"EnemiesManager: Manually removing enemy '{enemy.Race}'.");
                managedEnemy.CleanUp();
                _activeEnemies.Remove(enemy);
                return true;
            }
            return false;
        }

        public void ClearAllEnemies()
        {
            List<IEntity> enemiesToClear = new List<IEntity>(_activeEnemies.Keys);
            foreach (IEntity enemyKey in enemiesToClear)
            {
                if (_activeEnemies.TryGetValue(enemyKey, out ManagedEnemy managedEnemy))
                {
                    managedEnemy.CleanUp();
                }
            }
            _activeEnemies.Clear();
            GD.Print("EnemiesManager: All active enemies cleared.");
        }

        public Node2D GetEnemyNode(IEntity enemy)
        {
            if (enemy != null && _activeEnemies.TryGetValue(enemy, out ManagedEnemy managedEnemy))
            {
                return managedEnemy.NodeInstance;
            }
            return null;
        }

        public IEnumerable<IEntity> GetAllActiveEnemies()
        {
            return _activeEnemies.Keys.ToList();
        }
    }
}