using System;
using Godot;
using MAPZ_lab_RPG.Entities;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Scenes.Managers
{
    public class GameManagerFacade
    {
        public MainHeroManager MainHeroManager { get; private set; }
        public EnemiesManager EnemiesManager { get; private set; }

        private ICreature _selectedEnemyTarget;
        private Control _selectedEnemyVisual;
        private Control _previouslySelectedVisual;

        public Action EndBattle;
        public Action LoseBattle;

        public GameManagerFacade()
        {
            MainHeroManager = new MainHeroManager();
            EnemiesManager = new EnemiesManager();
        }

        public void SetNodes(Control heroDisplayNode, GridContainer enemyPlaceholderNode, Label moneyLabel, Label levelLabel, int level)
        {
            MainHeroManager.SetNodes(heroDisplayNode, moneyLabel, levelLabel);
            EnemiesManager.SetNodes(enemyPlaceholderNode, level);

            EnemiesManager.OnEnemyDefeated += HandleAnEnemyDefeated;
            EnemiesManager.OnAllEnemiesDefeated += HandleAllEnemiesDefeated;
            EnemiesManager.OnEnemyVisualClicked += HandleEnemyClicked;
        }

        private void HandleAllEnemiesDefeated()
        {
            GD.Print("BattleScene: VICTORY! All enemies are defeated.");
            _selectedEnemyTarget = null;
            _selectedEnemyVisual = null;
            _previouslySelectedVisual = null;

            MainHeroManager.EarnRewards(GameData.CurrentRound);

            GameData.CurrentRound++;
            EndBattle?.Invoke();
        }

        private void HandleEnemyClicked(ICreature enemy, Control visual)
        {
            if (!MainHeroManager.IsHeroAlive() || !EnemiesManager.HasActiveEnemies() || enemy.Health <= 0)
            {
                GD.Print("Cannot select target: Hero defeated, no active enemies, or target is already defeated.");
                return;
            }

            GD.Print($"BattleScene: Clicked! Target: {enemy.Name}");

            // Handle selection for clicked enemy visual
            // Remove selection highlight from previously selected visual
            if (_previouslySelectedVisual != null && _previouslySelectedVisual != visual)
            {
                _previouslySelectedVisual.Modulate = Colors.White;
            }

            // Set the new selected enemy and its visual
            _selectedEnemyTarget = enemy;
            _selectedEnemyVisual = visual;

            if (_selectedEnemyVisual != null)
            {
                _selectedEnemyVisual.Modulate = new Color(1.2f, 1.2f, 0.8f, 1.0f);
            }
            _previouslySelectedVisual = _selectedEnemyVisual;

            PerformPlayerAttack(_selectedEnemyTarget);
        }
        private void PerformPlayerAttack(ICreature targetEnemy)
        {
            if (targetEnemy == null || targetEnemy.Health <= 0)
            {
                GD.Print("Player attack: Invalid or already defeated target.");
                if (_selectedEnemyTarget == targetEnemy)
                {
                    if (_selectedEnemyVisual != null) _selectedEnemyVisual.Modulate = Colors.White;
                    _selectedEnemyTarget = null;
                    _selectedEnemyVisual = null;
                    _previouslySelectedVisual = null;
                }
                return;
            }
            if (!MainHeroManager.IsHeroAlive())
            {
                GD.Print("Player attack: Hero is defeated and cannot attack.");
                return;
            }

            double playerDamage = MainHeroManager.GetHeroAttackDamage();
            GD.Print($"Player attacks {targetEnemy.Name} for {playerDamage} potential damage.");
            EnemiesManager.ApplyDamageToEnemy(targetEnemy, playerDamage);

            if (targetEnemy.Health <= 0)
            {
                _selectedEnemyTarget = null;
                _selectedEnemyVisual = null;
                _previouslySelectedVisual = null;
            }

            if (EnemiesManager.HasActiveEnemies() && MainHeroManager.IsHeroAlive())
            {
                HandleEnemyTurns();
            }
        }
        private void HandleEnemyTurns()
        {
            if (!MainHeroManager.IsHeroAlive() || !EnemiesManager.HasActiveEnemies()) return;

            GD.Print("--- Enemy Turn Starts ---");
            List<ICreature> currentAttackers = new List<ICreature>(EnemiesManager.GetActiveEnemies());

            foreach (ICreature enemy in currentAttackers)
            {
                if (!MainHeroManager.IsHeroAlive()) break;
                if (enemy.Health <= 0) continue;

                double enemyDamage = EnemiesManager.GetEnemyAttackDamage(enemy);
                GD.Print($"{enemy.Name} attacks hero for {enemyDamage} damage.");
                MainHeroManager.HeroTakeDamage(enemyDamage);

                if (!MainHeroManager.IsHeroAlive())
                {
                    GD.Print("Hero has been defeated!");
                    LoseBattle?.Invoke();
                    break;
                }
            }
            GD.Print("--- Enemy Turn Ends ---");
        }
        private void HandleAnEnemyDefeated(ICreature defeatedEnemy)
        {
            GD.Print($"BattleScene: {defeatedEnemy.Name} was defeated!");
            if (_selectedEnemyTarget == defeatedEnemy)
            {
                _selectedEnemyTarget = null;
                _selectedEnemyVisual = null;
                _previouslySelectedVisual = null;
            }
        }
        public void Cleanup()
        {
            EnemiesManager.Cleanup();
        }
    }
}