using Godot;
using MAPZ_lab_RPG.Entities.Heroes;
using MAPZ_lab_RPG.Entities.Enemies;
using System.Collections.Generic;
using System.Linq;

public partial class Main : Node2D
{
	private MainHero _mainHero;
	private EnemyCreator _enemyCreator;
	private List<IEnemy> _currentEnemies;
	private IEnemy _currentEnemy;
	private Sprite2D _heroSprite;
	private Label _heroNameLabel;
	private ProgressBar _heroHPBar;
	private Node2D _enemiesContainer;
	private Dictionary<IEnemy, (Sprite2D sprite, Label nameLabel, ProgressBar hpBar)> _enemyNodes = new Dictionary<IEnemy, (Sprite2D sprite, Label nameLabel, ProgressBar hpBar)>();
	private int _currentLevel = 0;
	private bool _isCombatActive = false;
	private PackedScene _shopScene;
	private float _heroHP = 100f; // Simulated HP since MainHero doesn't expose health
	private List<(Sprite2D sprite, Label nameLabel, ProgressBar hpBar)> _placeholderEnemyNodes = new List<(Sprite2D sprite, Label nameLabel, ProgressBar hpBar)>();

	public override void _Ready()
	{
		// Initialize singletons
		_mainHero = MainHero.Instance;
		_enemyCreator = EnemyCreator.Instance;

		// Select a default hero
		_mainHero.HeroSelect("Archer");

		// Set up hero UI
		_heroSprite = GetNode<Sprite2D>("Hero");
		_heroNameLabel = GetNode<Label>("Hero/HeroName");
		_heroHPBar = GetNode<ProgressBar>("Hero/HeroHPBar");
		_heroNameLabel.Text = "Archer"; // Update if hero changes
		_heroHPBar.Value = _heroHP;

		// Set up enemies container
		_enemiesContainer = GetNode<Node2D>("EnemiesContainer");

		// Get placeholder enemy nodes
		GetPlaceholderEnemyNodes();

		// Load shop scene
		_shopScene = GD.Load<PackedScene>("res://Shop.tscn");

		// Start the first level
		StartNextLevel();
	}

	private void GetPlaceholderEnemyNodes()
	{
		_placeholderEnemyNodes.Clear();
		Node2D placeholdersNode = GetNode<Node2D>("EnemiesContainer/EnemyPlaceholders");
		if (placeholdersNode != null)
		{
			foreach (Node child in placeholdersNode.GetChildren())
			{
				if (child is Sprite2D sprite)
				{
					string baseName = sprite.Name.ToString().Replace("EnemyPlaceholder", "");
					Label nameLabel = GetNode<Label>("EnemiesContainer/EnemyPlaceholders/EnemyPlaceholder" + baseName + "Name");
					ProgressBar hpBar = GetNode<ProgressBar>("EnemiesContainer/EnemyPlaceholders/EnemyPlaceholder" + baseName + "HPBar");
					_placeholderEnemyNodes.Add((sprite, nameLabel, hpBar));
				}
			}
		}
	}

	public override void _Process(double delta)
	{
		if (!_isCombatActive) return;

		// Hero attacks enemy
		if (Input.IsActionJustPressed("ui_accept"))
		{
			float heroDamage = _mainHero.Atack();
			double enemyHealthAfterDamage = _currentEnemy.GetDamage(heroDamage);
			GD.Print($"Hero attacks {_currentEnemy.Race} for {heroDamage} damage! Enemy health: {enemyHealthAfterDamage}");
			UpdateEnemyHPBar(_currentEnemy);

			if (enemyHealthAfterDamage <= 0)
			{
				GD.Print($"{_currentEnemy.Race} defeated!");
				RemoveEnemyNode(_currentEnemy);
				_currentEnemies.Remove(_currentEnemy);
				if (_currentEnemies.Count == 0)
				{
					GD.Print($"Level {_currentLevel} cleared!");
					_mainHero.AddExperience(100 * _currentLevel); // Reward XP
					_mainHero.AddCoins(50 * _currentLevel); // Reward coins
					ShowShop();
				}
				else
				{
					SwitchToNextEnemy();
				}
			}
			else
			{
				// Enemy retaliates
				double enemyDamage = _currentEnemy.Attack();
				_heroHP -= (float)enemyDamage; // Simulated HP reduction
				_mainHero.GetDamage((float)enemyDamage);
				GD.Print($"{_currentEnemy.Race} attacks Hero for {enemyDamage} damage!");
				UpdateHeroHPBar();
			}
		}

		// Hero heals (for testing/survival)
		if (Input.IsActionJustPressed("ui_up"))
		{
			float healed = _mainHero.Heal(5f);
			_heroHP += healed; // Simulated HP increase
			if (_heroHP > 100f) _heroHP = 100f;
			GD.Print($"Hero heals for {healed} HP!");
			UpdateHeroHPBar();
		}
	}

	private void StartNextLevel()
	{
		_currentLevel++;
		GD.Print($"Starting Level {_currentLevel}");
		_currentEnemies = _enemyCreator.CreateEnemies(_currentLevel);
		if (_currentEnemies.Count == 0)
		{
			GD.Print("No enemies spawned! Game Over or Debug needed.");
			_isCombatActive = false;
			return;
		}
		ClearEnemies();
		SpawnEnemies();
		SwitchToNextEnemy();
	}

	private void SpawnEnemies()
	{
		_enemyNodes.Clear();

		//hide all placeholders
		foreach ((Sprite2D sprite, Label nameLabel, ProgressBar hpBar) in _placeholderEnemyNodes)
		{
			sprite.Visible = false;
			nameLabel.Visible = false;
			hpBar.Visible = false;
		}

		int maxCols = 3;
		float xStart = 0;
		float yStart = 0;
		float xSpacing = 150;
		float ySpacing = 200;

		// Calculate the center position of the EnemiesContainer.  We'll use the size of the Main node instead.
		Rect2 mainRect = GetViewportRect();
		Vector2 containerSize = mainRect.Size;
		xStart = containerSize.X / 2 - (maxCols - 1) * xSpacing / 2;
		yStart = containerSize.Y / 2 - (2 - 1) * ySpacing / 2;

		for (int i = 0; i < _currentEnemies.Count; i++)
		{
			IEnemy enemy = _currentEnemies[i];
			int row = i / maxCols;
			int col = i % maxCols;

			float xOffset = xStart + col * xSpacing;
			float yOffset = yStart + row * ySpacing;

			Sprite2D sprite;
			Label nameLabel;
			ProgressBar hpBar;

			if (i < _placeholderEnemyNodes.Count)
			{
				//use a placeholder
				(sprite, nameLabel, hpBar) = _placeholderEnemyNodes[i];
				sprite.Position = new Vector2(xOffset, yOffset); //update position
				sprite.Visible = true; //show
				nameLabel.Position = new Vector2(xOffset - 50, yOffset - 100);
				nameLabel.Text = enemy.Race;
				nameLabel.Visible = true;
				hpBar.Position = new Vector2(xOffset - 50, yOffset + 80);
				hpBar.MaxValue = enemy.Health;
				hpBar.Value = enemy.Health;
				hpBar.Visible = true;
			}
			else
			{
				// Create new if there are more enemies than placeholders.
				sprite = new Sprite2D();
				sprite.Texture = GD.Load<Texture2D>("res://icon.svg");  // Replace
				sprite.Position = new Vector2(xOffset, yOffset);
				sprite.Scale = new Vector2(0.5f, 0.5f);
				_enemiesContainer.AddChild(sprite);

				nameLabel = new Label();
				nameLabel.Text = enemy.Race;
				nameLabel.Position = new Vector2(xOffset - 50, yOffset - 100);
				nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
				_enemiesContainer.AddChild(nameLabel);

				hpBar = new ProgressBar();
				hpBar.Size = new Vector2(100, 20);
				hpBar.Position = new Vector2(xOffset - 50, yOffset + 80);
				hpBar.MaxValue = enemy.Health;
				hpBar.Value = enemy.Health;
				_enemiesContainer.AddChild(hpBar);
			}
			_enemyNodes[enemy] = (sprite, nameLabel, hpBar);
		}
	}

	private void SwitchToNextEnemy()
	{
		if (_currentEnemies.Count == 0) return;

		_currentEnemy = _currentEnemies[0];
		_isCombatActive = true;
		GD.Print($"Now fighting: {_currentEnemy.Race} (Health: {_currentEnemy.Health}, Damage: {_currentEnemy.Damage}, Armor: {_currentEnemy.Armor})");
	}

	private void ShowShop()
	{
		_isCombatActive = false;
		var shopInstance = _shopScene.Instantiate<Shop>();
		shopInstance.Connect("ShopClosed", new Callable(this, nameof(OnShopClosed)));
		AddChild(shopInstance);
	}

	private void OnShopClosed()
	{
		StartNextLevel();
	}

	private void UpdateHeroHPBar()
	{
		_heroHPBar.Value = _heroHP;
		if (_heroHP <= 0)
		{
			GD.Print("Hero defeated! Game Over.");
			_isCombatActive = false;
			// Add game over logic if needed
		}
	}

	private void UpdateEnemyHPBar(IEnemy enemy)
	{
		if (_enemyNodes.TryGetValue(enemy, out var nodes))
		{
			nodes.hpBar.Value = enemy.Health;
		}
	}

	private void RemoveEnemyNode(IEnemy enemy)
	{
		if (_enemyNodes.TryGetValue(enemy, out var nodes))
		{
			nodes.sprite.QueueFree();
			nodes.nameLabel.QueueFree();
			nodes.hpBar.QueueFree();
			_enemyNodes.Remove(enemy);
		}
	}

	private void ClearEnemies()
	{
		foreach (var enemy in _enemyNodes.Keys)
		{
			RemoveEnemyNode(enemy);
		}
		_enemyNodes.Clear();
		_currentEnemies.Clear();
	}
}
