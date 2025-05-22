// BattleScene.cs
using Godot;
using Scenes.Managers;
using MAPZ_lab_RPG.Entities;
using System.Collections.Generic;
using System;

public partial class BattleScene : Node2D // Or whatever your root node type is
{
	// Hero related
	private Control _heroDisplayNode; // Changed from Node2D to Control
	private MainHeroManager _mainHeroManager;

	// Enemy related
	private Control _enemyPlaceholderNode; // The parent where enemy visuals are added
	private EnemiesManager _enemiesManager;

	// UI Labels
	private Label _moneyLabel;
	private Label _levelLabel;

	// Targeting
	private IEntity _selectedEnemyTarget;
	private Control _selectedEnemyVisual; // To store the visual of the selected enemy
	private Control _previouslySelectedVisual; // For de-highlighting

	public override void _Ready()
	{
		// --- Hero Setup ---
		string selectedHero = GameData.SelectedHeroName ?? "Archer"; // Fallback for testing
		if (GameData.SelectedHeroName == null) GD.PrintRich("[color=yellow]BattleScene Warning: GameData.SelectedHeroName is null. Defaulting to 'Archer'.[/color]");

		// Instantiate Entity.tscn (root Control) for the hero display
		_heroDisplayNode = GD.Load<PackedScene>("res://Scenes/Entity.tscn").Instantiate<Control>();

		var heroContainer = GetNode<CenterContainer>("Control/VBoxContainer/HBoxContainer2/CenterContainer");
		if (heroContainer == null) { GD.PrintErr("BattleScene: Hero container node 'Control/VBoxContainer/HBoxContainer2/CenterContainer' not found!"); GetTree().Quit(); return; }
		heroContainer.AddChild(_heroDisplayNode);
		_heroDisplayNode.Name = "HeroDisplayNode";

		_moneyLabel = GetNode<Label>("Control/VBoxContainer/HBoxContainer3/MoneyLabel");
		_levelLabel = GetNode<Label>("Control/VBoxContainer/HBoxContainer3/LevelLabel");
		if (_moneyLabel == null || _levelLabel == null) { GD.PrintErr("BattleScene: MoneyLabel or LevelLabel not found!"); GetTree().Quit(); return; }

		// MainHeroManager constructor now correctly receives a Control node
		_mainHeroManager = new MainHeroManager(_heroDisplayNode, selectedHero, _moneyLabel, _levelLabel);
		GD.Print("MainHeroManager initialized.");

		// --- Enemies Setup ---
		_enemyPlaceholderNode = GetNode<CenterContainer>("Control/VBoxContainer/HBoxContainer2/CenterContainer2");
		if (_enemyPlaceholderNode == null) { GD.PrintErr("BattleScene: Enemy placeholder node 'Control/VBoxContainer/HBoxContainer2/CenterContainer2' not found!"); GetTree().Quit(); return; }

		int currentBattleLevel = 1; // Example level
		_enemiesManager = new EnemiesManager(_enemyPlaceholderNode, currentBattleLevel);
		GD.Print($"EnemiesManager initialized. Enemies placed under: {_enemyPlaceholderNode.GetPath()}");

		_enemiesManager.OnEnemyDefeated += HandleAnEnemyDefeated;
		_enemiesManager.OnAllEnemiesDefeated += HandleAllEnemiesDefeated;
		_enemiesManager.OnEnemyVisualClicked += HandleEnemyClicked; // Subscribe to click event

		if (!_enemiesManager.HasActiveEnemies() && _mainHeroManager.IsHeroAlive())
		{
			GD.Print("BattleScene: No enemies to fight from the start.");
			HandleAllEnemiesDefeated();
		}
	}

	private void HandleEnemyClicked(IEntity enemy, Control visual)
	{
		if (!_mainHeroManager.IsHeroAlive() || !_enemiesManager.HasActiveEnemies() || enemy.Health <= 0)
		{
			GD.Print("Cannot select target: Hero defeated, no active enemies, or target is already defeated.");
			return;
		}

		GD.Print($"BattleScene: Clicked! Target: {enemy.Race}");

		if (_previouslySelectedVisual != null && IsInstanceValid(_previouslySelectedVisual) && _previouslySelectedVisual != visual)
		{
			_previouslySelectedVisual.Modulate = Colors.White; // Reset tint of old selection
		}

		_selectedEnemyTarget = enemy;
		_selectedEnemyVisual = visual;

		if (_selectedEnemyVisual != null && IsInstanceValid(_selectedEnemyVisual))
		{
			_selectedEnemyVisual.Modulate = new Color(1.2f, 1.2f, 0.8f, 1.0f); // Highlight new selection
		}
		_previouslySelectedVisual = _selectedEnemyVisual;

		// --- Primary Action: Click-to-Attack ---
		PerformPlayerAttack(_selectedEnemyTarget);
	}

	private void PerformPlayerAttack(IEntity targetEnemy)
	{
		if (targetEnemy == null || targetEnemy.Health <= 0)
		{
			GD.Print("Player attack: Invalid or already defeated target.");
			if (_selectedEnemyTarget == targetEnemy) // Clear selection if it was this invalid target
			{
				if (_selectedEnemyVisual != null && IsInstanceValid(_selectedEnemyVisual)) _selectedEnemyVisual.Modulate = Colors.White;
				_selectedEnemyTarget = null;
				_selectedEnemyVisual = null;
				_previouslySelectedVisual = null;
			}
			return;
		}
		if (!_mainHeroManager.IsHeroAlive())
		{
			GD.Print("Player attack: Hero is defeated and cannot attack.");
			return;
		}

		double playerDamage = _mainHeroManager.GetHeroAttackDamage();
		GD.Print($"Player attacks {targetEnemy.Race} for {playerDamage} potential damage.");
		_enemiesManager.ApplyDamageToEnemy(targetEnemy, playerDamage);

		if (targetEnemy.Health <= 0) // Target was defeated by this attack
		{
			// _selectedEnemyVisual is now invalid as it was QueueFree'd by EnemiesManager
			_selectedEnemyTarget = null;
			_selectedEnemyVisual = null;
			_previouslySelectedVisual = null; // Clear this too
		}

		if (_enemiesManager.HasActiveEnemies() && _mainHeroManager.IsHeroAlive())
		{
			HandleEnemyTurns();
		}
	}

	private async void HandleEnemyTurns()
	{
		if (!_mainHeroManager.IsHeroAlive()) return;
		if (!_enemiesManager.HasActiveEnemies()) return;


		GD.Print("--- Enemy Turn Starts ---");
		// It's safer to iterate over a copy if the list might change (e.g. an enemy dies from a reflected attack)
		List<IEntity> currentAttackers = new List<IEntity>(_enemiesManager.GetActiveEnemies());

		foreach (IEntity enemy in currentAttackers)
		{
			if (!_mainHeroManager.IsHeroAlive()) break; // Stop if hero is defeated mid-turn
			if (enemy.Health <= 0) continue; // Skip if this enemy was defeated by a previous enemy's side effect (rare)

			double enemyDamage = _enemiesManager.GetEnemyAttackDamage(enemy);
			GD.Print($"{enemy.Race} attacks hero for {enemyDamage} damage.");
			_mainHeroManager.HeroTakeDamage(enemyDamage);

			// Optional: Small delay between enemy attacks for better flow
			await ToSignal(GetTree().CreateTimer(0.4f), SceneTreeTimer.SignalName.Timeout);

			if (!_mainHeroManager.IsHeroAlive())
			{
				GD.Print("Hero has been defeated!");
				// TODO: Implement Game Over logic (e.g., show a screen, offer retry)
				break;
			}
		}
		GD.Print("--- Enemy Turn Ends ---");
	}

	private void HandleAnEnemyDefeated(IEntity defeatedEnemy)
	{
		GD.Print($"BattleScene: {defeatedEnemy.Race} was defeated!");
		if (_selectedEnemyTarget == defeatedEnemy)
		{
			_selectedEnemyTarget = null;
			_selectedEnemyVisual = null; // Visual is gone
			_previouslySelectedVisual = null;
		}
		// TODO: Add XP, loot, etc.
	}

	private void HandleAllEnemiesDefeated()
	{
		GD.Print("BattleScene: VICTORY! All enemies are defeated.");
		_selectedEnemyTarget = null;
		_selectedEnemyVisual = null;
		_previouslySelectedVisual = null;

		Label victoryLabel = new Label
		{
			Text = "VICTORY!",
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			CustomMinimumSize = new Vector2(200,50) // Give it some size
		};
		victoryLabel.SetAnchorsPreset(Control.LayoutPreset.Center); // Center it

		// Add to a high-level UI layer. "Control" is the root of your UI in BattleScene.tscn
		var uiRoot = GetNodeOrNull<Control>("Control");
		if (uiRoot != null)
		{
			uiRoot.AddChild(victoryLabel);
		} else {
			AddChild(victoryLabel); // Fallback, might not be ideal for layering
			GD.PrintErr("Could not find 'Control' node to add VictoryLabel. Added to BattleScene root.");
		}
		// Example: GetTree().ChangeSceneToFile("res://Scenes/WorldMap.tscn");
	}

	public override void _ExitTree()
	{
		if (_enemiesManager != null)
		{
			_enemiesManager.OnEnemyDefeated -= HandleAnEnemyDefeated;
			_enemiesManager.OnAllEnemiesDefeated -= HandleAllEnemiesDefeated;
			_enemiesManager.OnEnemyVisualClicked -= HandleEnemyClicked;
			_enemiesManager.Cleanup();
		}
	}
}
