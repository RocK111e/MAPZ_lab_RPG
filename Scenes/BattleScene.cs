using Godot;
using Scenes.Managers;
using MAPZ_lab_RPG.Entities;
using System.Collections.Generic;
using System;
using MAPZ_lab_RPG.Entities.Heroes;

public partial class BattleScene : Node2D
{
	private Control _heroDisplayNode; 
	private MainHeroManager _mainHeroManager;

	private GridContainer _enemyPlaceholderNode; 
	private EnemiesManager _enemiesManager;

	private Label _moneyLabel;
	private Label _levelLabel;

	private IEntity _selectedEnemyTarget;
	private Control _selectedEnemyVisual;
	private Control _previouslySelectedVisual;

	public override void _Ready()
	{
		var RoundLabel = GetNode<Label>("Control/VBoxContainer/HBoxContainer/RoundLabel");
		RoundLabel.Text = $"Round: {GameData.CurrentRound}";
	
		string selectedHero = MainHero.Instance.HeroEnumToStr(GameData.SelectedHeroName);

		_heroDisplayNode = GD.Load<PackedScene>("res://Scenes/Entity.tscn").Instantiate<Control>();
		var heroContainer = GetNode<CenterContainer>("Control/VBoxContainer/HBoxContainer2/CenterContainer"); 
		if (heroContainer == null) { GD.PrintErr("BattleScene: Hero container node 'Control/VBoxContainer/HBoxContainer2/CenterContainer' not found!"); GetTree().Quit(); return; }
		heroContainer.AddChild(_heroDisplayNode); 
		_heroDisplayNode.Name = "HeroDisplayNode";

		_moneyLabel = GetNode<Label>("Control/VBoxContainer/HBoxContainer3/MoneyLabel");
		_levelLabel = GetNode<Label>("Control/VBoxContainer/HBoxContainer3/LevelLabel");
		if (_moneyLabel == null || _levelLabel == null) { GD.PrintErr("BattleScene: MoneyLabel or LevelLabel not found!"); GetTree().Quit(); return; }
		_mainHeroManager = GameData.MainHeroManager ?? new MainHeroManager(_heroDisplayNode, GameData.SelectedHeroName, _moneyLabel, _levelLabel);
		GD.Print("MainHeroManager initialized.");
		GameData.MainHeroManager = _mainHeroManager;
		_mainHeroManager.SetNodes(_heroDisplayNode, _moneyLabel, _levelLabel);

		_enemyPlaceholderNode = GetNode<GridContainer>("Control/VBoxContainer/HBoxContainer2/GridContainer");
		if (_enemyPlaceholderNode == null) { GD.PrintErr("BattleScene: EnemyGridContainer node at 'Control/VBoxContainer/HBoxContainer2/EnemyGridContainer' not found! Ensure path is correct and type is GridContainer."); GetTree().Quit(); return; }

	

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

		GD.Print($"BattleScene: Clicked! Target: {enemy.Name}");

		if (_previouslySelectedVisual != null && IsInstanceValid(_previouslySelectedVisual) && _previouslySelectedVisual != visual)
		{
			_previouslySelectedVisual.Modulate = Colors.White;
		}

		_selectedEnemyTarget = enemy;
		_selectedEnemyVisual = visual;

		if (_selectedEnemyVisual != null && IsInstanceValid(_selectedEnemyVisual))
		{
			_selectedEnemyVisual.Modulate = new Color(1.2f, 1.2f, 0.8f, 1.0f);
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
		GD.Print($"Player attacks {targetEnemy.Name} for {playerDamage} potential damage.");
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
			GD.Print($"{enemy.Name} attacks hero for {enemyDamage} damage.");
			_mainHeroManager.HeroTakeDamage(enemyDamage);

			await ToSignal(GetTree().CreateTimer(0.4f), SceneTreeTimer.SignalName.Timeout);

			if (!_mainHeroManager.IsHeroAlive())
			{
				GD.Print("Hero has been defeated!");
				GetTree().ChangeSceneToFile("res://Scenes/LoseScreen.tscn");
				break;
			}
		}
		GD.Print("--- Enemy Turn Ends ---");
	}

	private void HandleAnEnemyDefeated(IEntity defeatedEnemy)
	{
		GD.Print($"BattleScene: {defeatedEnemy.Name} was defeated!");
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

		int coinsForRound = HandleCoinsReward();
		int expirienceForRound = HandleExpirienceReward();

		_mainHeroManager.GainCoins(coinsForRound);
		_mainHeroManager.AddExperienceAndLevelUpCheck(expirienceForRound);

		GameData.CurrentRound++;
		GetTree().ChangeSceneToFile("res://Scenes/Shop.tscn");
	}

	private int HandleCoinsReward()
	{
		int coinsForRound = 50 + 5 * GameData.CurrentRound;
		return coinsForRound;
	}

	private int HandleExpirienceReward()
	{
		int expirienceForRound = 30 + 20 * GameData.CurrentRound;
		return expirienceForRound;
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
