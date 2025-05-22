using Godot;
using Scenes.Managers;
using MAPZ_lab_RPG.Entities;
using System.Collections.Generic;
using System;

public partial class BattleScene : Node2D
{
	// Hero related
	private Control _heroDisplayNode; // Root of Entity.tscn for hero
	private MainHeroManager _mainHeroManager;

	// Enemy related
	private GridContainer _enemyPlaceholderNode; // This is the GridContainer for enemies
	private EnemiesManager _enemiesManager;

	// UI Labels
	private Label _moneyLabel;
	private Label _levelLabel;

	// Targeting
	private IEntity _selectedEnemyTarget;
	private Control _selectedEnemyVisual;
	private Control _previouslySelectedVisual;

	public override void _Ready()
	{
		var RoundLabel = GetNode<Label>("Control/VBoxContainer/HBoxContainer/RoundLabel");
		RoundLabel.Text = $"Round: {GameData.CurrentRound}";
		// --- Hero Setup ---
		string selectedHero = GameData.SelectedHeroName;
		if (GameData.SelectedHeroName == null) GD.PrintRich("[color=yellow]BattleScene Warning: GameData.SelectedHeroName is null. Defaulting to 'Archer'.[/color]");

		_heroDisplayNode = GD.Load<PackedScene>("res://Scenes/Entity.tscn").Instantiate<Control>();
		var heroContainer = GetNode<CenterContainer>("Control/VBoxContainer/HBoxContainer2/CenterContainer"); // Hero's parent
		if (heroContainer == null) { GD.PrintErr("BattleScene: Hero container node 'Control/VBoxContainer/HBoxContainer2/CenterContainer' not found!"); GetTree().Quit(); return; }
		heroContainer.AddChild(_heroDisplayNode); // Hero added to its CenterContainer
		_heroDisplayNode.Name = "HeroDisplayNode";

		_moneyLabel = GetNode<Label>("Control/VBoxContainer/HBoxContainer3/MoneyLabel");
		_levelLabel = GetNode<Label>("Control/VBoxContainer/HBoxContainer3/LevelLabel");
		if (_moneyLabel == null || _levelLabel == null) { GD.PrintErr("BattleScene: MoneyLabel or LevelLabel not found!"); GetTree().Quit(); return; }
		_mainHeroManager = GameData.MainHeroManager ?? new MainHeroManager(_heroDisplayNode, selectedHero, _moneyLabel, _levelLabel);
		GD.Print("MainHeroManager initialized.");
		GameData.MainHeroManager = _mainHeroManager;
		_mainHeroManager.SetNodes(_heroDisplayNode, _moneyLabel, _levelLabel);

		// --- Enemies Setup ---
		// Path to the GridContainer (previously CenterContainer2, now changed type and renamed)
		_enemyPlaceholderNode = GetNode<GridContainer>("Control/VBoxContainer/HBoxContainer2/GridContainer");
		if (_enemyPlaceholderNode == null) { GD.PrintErr("BattleScene: EnemyGridContainer node at 'Control/VBoxContainer/HBoxContainer2/EnemyGridContainer' not found! Ensure path is correct and type is GridContainer."); GetTree().Quit(); return; }

		// Configure GridContainer columns in the Godot Editor inspector for EnemyGridContainer.
		// Example: _enemyPlaceholderNode.Columns = 3; // Or set this in the editor.

		int currentBattleLevel = GameData.CurrentRound;
		_enemiesManager = new EnemiesManager(_enemyPlaceholderNode, currentBattleLevel);
		GD.Print($"EnemiesManager initialized. Enemies placed into GridContainer: {_enemyPlaceholderNode.GetPath()}");

		_enemiesManager.OnEnemyDefeated += HandleAnEnemyDefeated;
		_enemiesManager.OnAllEnemiesDefeated += HandleAllEnemiesDefeated;
		_enemiesManager.OnEnemyVisualClicked += HandleEnemyClicked;

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
			_previouslySelectedVisual.Modulate = Colors.White;
		}

		_selectedEnemyTarget = enemy;
		_selectedEnemyVisual = visual;

		if (_selectedEnemyVisual != null && IsInstanceValid(_selectedEnemyVisual))
		{
			_selectedEnemyVisual.Modulate = new Color(1.2f, 1.2f, 0.8f, 1.0f); // Highlight
		}
		_previouslySelectedVisual = _selectedEnemyVisual;

		PerformPlayerAttack(_selectedEnemyTarget);
	}

	private void PerformPlayerAttack(IEntity targetEnemy)
	{
		if (targetEnemy == null || targetEnemy.Health <= 0)
		{
			GD.Print("Player attack: Invalid or already defeated target.");
			if (_selectedEnemyTarget == targetEnemy)
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

		if (targetEnemy.Health <= 0)
		{
			_selectedEnemyTarget = null;
			_selectedEnemyVisual = null;
			_previouslySelectedVisual = null;
		}

		if (_enemiesManager.HasActiveEnemies() && _mainHeroManager.IsHeroAlive())
		{
			HandleEnemyTurns();
		}
	}

	private async void HandleEnemyTurns()
	{
		if (!_mainHeroManager.IsHeroAlive() || !_enemiesManager.HasActiveEnemies()) return;

		GD.Print("--- Enemy Turn Starts ---");
		List<IEntity> currentAttackers = new List<IEntity>(_enemiesManager.GetActiveEnemies());

		foreach (IEntity enemy in currentAttackers)
		{
			if (!_mainHeroManager.IsHeroAlive()) break;
			if (enemy.Health <= 0) continue;

			double enemyDamage = _enemiesManager.GetEnemyAttackDamage(enemy);
			GD.Print($"{enemy.Race} attacks hero for {enemyDamage} damage.");
			_mainHeroManager.HeroTakeDamage(enemyDamage);

			await ToSignal(GetTree().CreateTimer(0.4f), SceneTreeTimer.SignalName.Timeout);

			if (!_mainHeroManager.IsHeroAlive())
			{
				GD.Print("Hero has been defeated!");
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
			_selectedEnemyVisual = null;
			_previouslySelectedVisual = null;
		}
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
			CustomMinimumSize = new Vector2(200, 50)
		};
		victoryLabel.SetAnchorsPreset(Control.LayoutPreset.Center);

		var uiRoot = GetNodeOrNull<Control>("Control");
		if (uiRoot != null) uiRoot.AddChild(victoryLabel);
		else
		{
			AddChild(victoryLabel);
			GD.PrintErr("Could not find 'Control' node to add VictoryLabel. Added to BattleScene root.");
		}

		GameData.CurrentRound++;
		GetTree().ChangeSceneToFile("res://Scenes/Shop.tscn");
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
