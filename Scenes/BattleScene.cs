// BattleScene.cs
using Godot;
using Scenes.Managers;
using MAPZ_lab_RPG.Entities; // Required for IEntity if you use it directly in BattleScene
using System.Collections.Generic; // For List<IEntity>
using System; // For Random (optional, if used directly here)

public partial class BattleScene : Node2D // Or Control, or whatever your root node type is
{
	// Hero related nodes and manager
	private Node2D _heroDisplayNode;
	private MainHeroManager _mainHeroManager;

	// Enemy related nodes and manager
	// Corrected type for the placeholder node
	private Control _enemyPlaceholderNode; // CenterContainer is a Control.
	private EnemiesManager _enemiesManager;

	// UI Labels
	private Label _moneyLabel;
	private Label _levelLabel;

	// Example: For turn management or triggering actions
	private Button _attackButton;

	public override void _Ready()
	{
		// --- Hero Setup ---
		// Ensure GameData.SelectedHeroName is set before this scene loads
		string selectedHero = GameData.SelectedHeroName ?? "Archer"; // Fallback if null for testing
		if (GameData.SelectedHeroName == null)
		{
			GD.PrintRich("[color=yellow]BattleScene Warning: GameData.SelectedHeroName is null. Defaulting to 'Archer'.[/color]");
		}


		_heroDisplayNode = GD.Load<PackedScene>("res://Scenes/Entity.tscn").Instantiate<Node2D>();

		var heroContainer = GetNode<CenterContainer>("Control/VBoxContainer/HBoxContainer2/CenterContainer");
		if (heroContainer == null)
		{
			GD.PrintErr("BattleScene: Hero container node 'Control/VBoxContainer/HBoxContainer2/CenterContainer' not found! Ensure the path is correct in your scene tree.");
			GetTree().Quit(); // Critical error, can't proceed
			return;
		}
		heroContainer.AddChild(_heroDisplayNode);
		_heroDisplayNode.Name = "HeroDisplayNode";

		_moneyLabel = GetNode<Label>("Control/VBoxContainer/HBoxContainer3/MoneyLabel");
		_levelLabel = GetNode<Label>("Control/VBoxContainer/HBoxContainer3/LevelLabel");

		if (_moneyLabel == null || _levelLabel == null)
		{
			GD.PrintErr("BattleScene: MoneyLabel or LevelLabel not found! Ensure paths are correct.");
			GetTree().Quit(); // Critical error
			return;
		}

		_mainHeroManager = new MainHeroManager(_heroDisplayNode, selectedHero, _moneyLabel, _levelLabel);
		GD.Print("MainHeroManager initialized.");

		// --- Enemies Setup ---
		// Corrected GetNode<T> type to match the actual node type in the scene
		_enemyPlaceholderNode = GetNode<CenterContainer>("Control/VBoxContainer/HBoxContainer2/CenterContainer2");
		// Ensure "CenterContainer2" is indeed a CenterContainer in your scene.
		// If it's a different type of Control or Node2D, adjust GetNode<T> and the field type accordingly.

		if (_enemyPlaceholderNode == null)
		{
			GD.PrintErr("BattleScene: Enemy placeholder node 'Control/VBoxContainer/HBoxContainer2/CenterContainer2' not found! Ensure the path is correct.");
			GetTree().Quit(); // Critical error
			return;
		}

		int currentBattleLevel = 1; // Example level, make this dynamic later
		_enemiesManager = new EnemiesManager(_enemyPlaceholderNode, currentBattleLevel);
		GD.Print($"EnemiesManager initialized for level {currentBattleLevel}. Enemies placed under: {_enemyPlaceholderNode.GetPath()}");

		_enemiesManager.OnEnemyDefeated += HandleAnEnemyDefeated;
		_enemiesManager.OnAllEnemiesDefeated += HandleAllEnemiesDefeated;

		// --- UI & Interaction Setup (Example) ---
		// Ensure you have an "AttackButton" node at this path or update the path.
		_attackButton = GetNodeOrNull<Button>("Control/AttackButton");
		if (_attackButton != null)
		{
			_attackButton.Pressed += OnAttackButtonPressed;
			_attackButton.Disabled = !_enemiesManager.HasActiveEnemies();
		}
		else
		{
			GD.PrintRich("[color=yellow]BattleScene Warning: AttackButton node at 'Control/AttackButton' not found. Player attack via UI will not be available.[/color]");
		}

		if (!_enemiesManager.HasActiveEnemies() && _mainHeroManager.IsHeroAlive()) // If hero is alive but no enemies
		{
			 GD.Print("BattleScene: No enemies to fight from the start.");
			 HandleAllEnemiesDefeated(); // Or some other logic for instant win/no combat
		}
	}

	private void OnAttackButtonPressed()
	{
		if (_mainHeroManager == null || _enemiesManager == null)
		{
			GD.PrintErr("Attack pressed but managers not initialized!");
			return;
		}

		if (!_mainHeroManager.IsHeroAlive())
		{
			GD.Print("Player is defeated and cannot attack.");
			if(_attackButton != null) _attackButton.Disabled = true;
			return;
		}

		if (_enemiesManager.HasActiveEnemies())
		{
			List<IEntity> activeEnemies = _enemiesManager.GetActiveEnemies();
			if (activeEnemies.Count > 0)
			{
				IEntity targetEnemy = activeEnemies[0]; // Simple targeting: first enemy
				double playerDamage = _mainHeroManager.GetHeroAttackDamage();

				GD.Print($"Player attacks {targetEnemy.Race} for {playerDamage} potential damage.");
				_enemiesManager.ApplyDamageToEnemy(targetEnemy, playerDamage);

				// If the attacked enemy (or any enemy) is still alive, it's their turn.
				if (_enemiesManager.HasActiveEnemies()) // Check again after player's attack
				{
					// Wait a brief moment for player attack animation/feedback (optional)
					// await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
					HandleEnemyTurns();
				}
				// If all enemies were defeated by this attack, OnAllEnemiesDefeated will be triggered.
			}
		}
		else
		{
			GD.Print("No enemies left to attack!");
			if (_attackButton != null) _attackButton.Disabled = true;
		}
	}

	private void HandleEnemyTurns()
	{
		if (_mainHeroManager == null || _enemiesManager == null) return;
		if (!_mainHeroManager.IsHeroAlive()) return; // Hero already defeated

		GD.Print("--- Enemy Turn Starts ---");
		List<IEntity> activeEnemies = _enemiesManager.GetActiveEnemies(); // Get a fresh list
		foreach (IEntity enemy in activeEnemies)
		{
			if (!_mainHeroManager.IsHeroAlive()) break; // Stop if hero is defeated mid-turn

			double enemyDamage = _enemiesManager.GetEnemyAttackDamage(enemy);
			GD.Print($"{enemy.Race} attacks hero for {enemyDamage} damage.");
			_mainHeroManager.HeroTakeDamage(enemyDamage);

			if (!_mainHeroManager.IsHeroAlive())
			{
				GD.Print("Hero has been defeated!");
				if(_attackButton != null) _attackButton.Disabled = true;
				// TODO: Implement Game Over logic (e.g., show a screen, offer retry)
				break;
			}
			// Optional: Add a small delay between enemy attacks for better readability
			// await ToSignal(GetTree().CreateTimer(0.3f), SceneTreeTimer.SignalName.Timeout);
		}
		GD.Print("--- Enemy Turn Ends ---");

		// Re-enable attack button if hero is alive and enemies still exist
		if (_attackButton != null && _mainHeroManager.IsHeroAlive() && _enemiesManager.HasActiveEnemies())
		{
			_attackButton.Disabled = false;
		}
	}

	private void HandleAnEnemyDefeated(IEntity defeatedEnemy)
	{
		GD.Print($"BattleScene: {defeatedEnemy.Race} was defeated! Player might get XP/loot.");
		// Example: _mainHeroManager.AddExperienceAndLevelUpCheck(defeatedEnemy.GetXPReward());
		// Example: _mainHeroManager.GainCoins(defeatedEnemy.GetGoldDrop());

		if (_attackButton != null)
		{
			_attackButton.Disabled = !_enemiesManager.HasActiveEnemies();
		}
	}

	private void HandleAllEnemiesDefeated()
	{
		GD.Print("BattleScene: VICTORY! All enemies are defeated.");
		if (_attackButton != null)
		{
			_attackButton.Disabled = true;
		}

		// TODO: Implement Victory logic (e.g., show victory screen, rewards, transition to next scene)
		Label victoryLabel = new Label();
		victoryLabel.Text = "VICTORY!";
		victoryLabel.HorizontalAlignment = HorizontalAlignment.Center;
		victoryLabel.VerticalAlignment = VerticalAlignment.Center;
		victoryLabel.SetAnchorsPreset(Control.LayoutPreset.FullRect); // Make it fill its parent
		// Add to a high-level UI layer or a specific victory screen container
		// For simplicity, adding to the root of BattleScene's UI control
		var uiRoot = GetNodeOrNull<Control>("Control");
		if (uiRoot != null)
		{
			uiRoot.AddChild(victoryLabel);
		} else {
			AddChild(victoryLabel); // Fallback
		}
		// Example: GetTree().ChangeSceneToFile("res://Scenes/WorldMap.tscn");
	}

	public override void _ExitTree()
	{
		if (_enemiesManager != null)
		{
			_enemiesManager.OnEnemyDefeated -= HandleAnEnemyDefeated;
			_enemiesManager.OnAllEnemiesDefeated -= HandleAllEnemiesDefeated;
			_enemiesManager.Cleanup();
		}

		if (_attackButton != null && IsInstanceValid(_attackButton))
		{
			if (_attackButton.IsConnected(Button.SignalName.Pressed, Callable.From(OnAttackButtonPressed)))
			{
				_attackButton.Pressed -= OnAttackButtonPressed;
			}
		}
	}
}
