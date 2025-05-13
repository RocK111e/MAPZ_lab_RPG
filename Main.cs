// Main.cs
using Godot;
using System;
using System.Collections.Generic;
using System.Linq; // Needed for LINQ checks like Any() or All()
using MAPZ_lab_RPG.Entities.Heroes;
using MAPZ_lab_RPG.Entities.Enemies;
using MAPZ_lab_RPG.Entities.Items;

public partial class Main : Node // Ensure it doesn't have a namespace or matches Godot's expectation
{
	#region Node References
	// Views
	private CanvasLayer _combatView;
	private CanvasLayer _shopView;

	// Combat UI Elements
	private Label _heroHPLabel;
	private Label _heroArmorLabel;
	private Label _heroCoinsLabel; // In combat view (might be redundant)
	private Label _heroLevelLabel;
	private Node2D _enemyArea;
	private Label _turnIndicatorLabel;
	private Button _attackButton; // Reference needed if we disable/enable it
	private Button _endTurnButton; // Reference needed if we disable/enable it

	// Shop UI Elements
	private VBoxContainer _shopItemList;
	private Label _shopHeroCoinsLabel;
	private PackedScene _shopItemEntryTemplate; // To instance shop items

	// Enemy Scene Templates (Load these based on enemy type)
	// You might use a Dictionary for cleaner access
	private Dictionary<string, PackedScene> _enemyPrefabs = new Dictionary<string, PackedScene>();

	// Temporary storage for linking scene nodes to enemy data
	private Dictionary<Node, IEnemy> _enemyNodes = new Dictionary<Node, IEnemy>();
	#endregion

	#region Game State
	private enum GameState { PreGame, Combat, Shop, GameOver }
	private GameState currentState = GameState.PreGame;
	private int currentLevel = 0;
	private List<IEnemy> currentEnemies = new List<IEnemy>();
	private List<IItem> shopItems = new List<IItem>();
	private Dictionary<string, int> itemPrices = new Dictionary<string, int>();
	#endregion

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Main scene starting...");
		currentState = GameState.PreGame;

		// --- Get Node References ---
		try
		{
			_combatView = GetNode<CanvasLayer>("CombatView");
			_shopView = GetNode<CanvasLayer>("ShopView");

			// Combat Nodes
			_heroHPLabel = GetNode<Label>("CombatView/HeroDisplay/HeroStatusUI/HeroHPLabel");
			_heroArmorLabel = GetNode<Label>("CombatView/HeroDisplay/HeroStatusUI/HeroArmorLabel");
			_heroCoinsLabel = GetNode<Label>("CombatView/HeroDisplay/HeroStatusUI/HeroCoinsLabel"); // Might remove later
			_heroLevelLabel = GetNode<Label>("CombatView/HeroDisplay/HeroStatusUI/HeroLevelLabel");
			_enemyArea = GetNode<Node2D>("CombatView/EnemyArea");
			_turnIndicatorLabel = GetNode<Label>("CombatView/CombatUI/TurnIndicatorLabel");
			_attackButton = GetNode<Button>("CombatView/CombatUI/HBoxContainer/AttackButton");
			_endTurnButton = GetNode<Button>("CombatView/CombatUI/HBoxContainer/EndTurnButton");


			// Shop Nodes
			_shopItemList = GetNode<VBoxContainer>("ShopView/ItemList");
			_shopHeroCoinsLabel = GetNode<Label>("ShopView/HeroShopCoinsLabel");
			// Load the template scene for shop items (adjust path if needed)
			_shopItemEntryTemplate = GD.Load<PackedScene>("res://MainScene.tscn::ShopItemEntry"); // Special syntax to load internal node as scene
			if (_shopItemEntryTemplate == null) GD.PrintErr("Failed to load ShopItemEntry template!");


			// Load Enemy Prefabs (Adjust paths as needed)
			// Use try-catch or check Load result for robustness
			_enemyPrefabs.Add("Ork", GD.Load<PackedScene>("res://Scenes/Enemies/Ork.tscn")); // Example path
			_enemyPrefabs.Add("Goblin", GD.Load<PackedScene>("res://Scenes/Enemies/Goblin.tscn")); // Example path
			_enemyPrefabs.Add("Troll", GD.Load<PackedScene>("res://Scenes/Enemies/Troll.tscn")); // Example path
			// Add checks here to ensure scenes loaded correctly


			GD.Print("Node references obtained.");
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Error getting node references: {ex.Message}");
			GetTree().Quit(); // Critical error if UI nodes are missing
			return;
		}

		// Define Item Prices
		itemPrices.Add("Health Rune", 50);
		itemPrices.Add("Damage Rune", 75);
		itemPrices.Add("Armor Rune", 60);

		// --- Hero Setup ---
		var heroInstance = MainHero.Instance;
		string chosenHero = "Swordsman"; // Example
		try
		{
			heroInstance.HeroSelect(chosenHero);
			heroInstance.AddCoins(100); // Starting coins
			GD.Print($"Selected Hero: {chosenHero}.");
			// TODO: Update Hero UI immediately if Getters exist
			// UpdateHeroStatusUI(); // Call this if MainHero provides Getters
		}
		catch (Exception ex) { /* ... error handling ... */ GD.PrintErr($"Hero setup failed: {ex.Message}"); GetTree().Quit(); return; }

		// --- Start the First Level ---
		StartNewLevel(); // This will set state to Combat and update UI

		GD.Print("Main scene ready. First level starting.");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Remove simulation input later, rely on button signals
		// if (currentState == GameState.Combat && Input.IsActionJustPressed("ui_accept")) { EndCombatPhase(); }
		// else if (currentState == GameState.Shop) { // ... shop simulation input ... }
	}

	#region State Management & Transitions

	public void StartNewLevel()
	{
		currentLevel++;
		GD.Print($"--- Starting Level {currentLevel} ---");
		currentState = GameState.Combat;

		// Clear old enemy visuals
		ClearEnemyVisuals();

		// Create logical enemies
		var enemyCreatorInstance = EnemyCreator.Instance;
		try
		{
			currentEnemies = enemyCreatorInstance.CreateEnemies(currentLevel);
			GD.Print($"Created {currentEnemies.Count} enemies for level {currentLevel}.");

			// Create enemy visuals
			CreateEnemyVisuals();
		}
		catch (Exception ex) { GD.PrintErr($"Error creating enemies: {ex.Message}"); currentEnemies.Clear(); }

		// Update UI for Combat state
		_combatView.Show();
		_shopView.Hide();
		UpdateHeroStatusUI(); // Update labels
		UpdateCombatUI();     // Update turn indicator, enemy HP, etc.
		StartHeroTurn();      // Set up for the player's turn
	}

	public void EndCombatPhase()
	{
		if (currentState != GameState.Combat) return;
		GD.Print($"--- Level {currentLevel} Cleared! ---");

		// Grant Rewards (as before)
		var heroInstance = MainHero.Instance;
		int experienceGained = 50 * currentLevel;
		int coinsGained = 25 * currentLevel;
		heroInstance.AddExperience(experienceGained);
		int totalCoins = heroInstance.AddCoins(coinsGained);
		GD.Print($"Rewards: +{experienceGained} XP, +{coinsGained} Coins. Total Coins: {totalCoins}"); // Use returned value

		EnterShopPhase();
	}

	public void EnterShopPhase()
	{
		currentState = GameState.Shop;
		GD.Print("--- Entering Shop ---");
		shopItems.Clear();

		// Populate shopItems list (as before)
		var itemManager = ItemManager.Instance;
		try
		{
			shopItems.Add(itemManager.CreateItem("HealthRune"));
			shopItems.Add(itemManager.CreateItem("DamageRune"));
			shopItems.Add(itemManager.CreateItem("ArmorRune"));
		}
		catch (Exception ex) { GD.PrintErr($"Error creating shop items: {ex.Message}"); }

		// Update UI for Shop state
		_combatView.Hide();
		_shopView.Show();
		UpdateShopUI(); // Populate item list and update coin display
	}

	 public void ExitShopPhase()
	{
		if (currentState != GameState.Shop) return;
		GD.Print("--- Exiting Shop ---");
		StartNewLevel(); // Proceed to the next combat level
	}

		  public void GameOver()
	 {
		GD.Print("--- Game Over ---");
		currentState = GameState.GameOver;
		_combatView.Hide();
		_shopView.Hide();

		// Show a dedicated Game Over screen/UI element
		// Add a simple label for now:
		var gameOverLabel = new Label();
		gameOverLabel.Text = "GAME OVER";
		// Center the text within the label
		gameOverLabel.HorizontalAlignment = HorizontalAlignment.Center;
		gameOverLabel.VerticalAlignment = VerticalAlignment.Center;
		// Make the label itself fill the screen
		gameOverLabel.Size = GetViewport().GetVisibleRect().Size; // Correct way to get viewport size
		// Optional: Add styling like a larger font size
		// gameOverLabel.AddThemeFontSizeOverride("font_size", 48);
		AddChild(gameOverLabel);
		// Disable input or provide restart option
	 }

	#endregion

	#region UI Update Methods

	/// <summary>
	/// Updates hero status labels (HP, Armor, Coins, Level).
	/// Assumes MainHero.Instance has methods to get these values.
	/// Needs IHero interface and implementing classes to be updated.
	/// </summary>
	private void UpdateHeroStatusUI()
	{
		var hero = MainHero.Instance;
		// --- PLACEHOLDER --- Need actual Getters in MainHero/IHero ---
		// Example using hypothetical getters:
		// _heroHPLabel.Text = $"HP: {hero.GetHealth():F0} / {hero.GetMaxHealth():F0}";
		// _heroArmorLabel.Text = $"Armor: {hero.GetArmor():F1}";
		// _heroCoinsLabel.Text = $"Coins: {hero.GetCoins()}"; // For combat view label
		// _heroLevelLabel.Text = $"Level: {hero.GetLevel()}";
		// --- END PLACEHOLDER ---

		// Temporary update using only AddCoins return value for coins
		_heroCoinsLabel.Text = "Coins: ???"; // Update if hero.GetCoins() exists
		_heroHPLabel.Text = "HP: ??? / ???"; // Update if hero.GetHealth/Max exists
		_heroArmorLabel.Text = "Armor: ???"; // Update if hero.GetArmor exists
		_heroLevelLabel.Text = $"Level: {currentLevel}"; // Use game level for now

		 // Update shop coin display too if possible
		 _shopHeroCoinsLabel.Text = "Your Coins: ???"; // Update if hero.GetCoins() exists
	}

	/// <summary>
	/// Updates UI elements specific to the combat view (turn indicator, enemy HP bars).
	/// </summary>
	   /// <summary>
	/// Updates UI elements specific to the combat view (turn indicator, enemy HP bars).
	/// </summary>
	private void UpdateCombatUI()
	{
		// Update turn indicator based on who's turn it is (managed elsewhere)
		// _turnIndicatorLabel.Text = isHeroTurn ? "Hero Turn" : "Enemy Turn";

		// Update Enemy HP displays
		foreach (var kvp in _enemyNodes)
		{
			Node enemyNode = kvp.Key;
			IEnemy enemyData = kvp.Value;
			// Find the HP label within the enemy node's scene structure
			// Adjust the path "StatusUI/HPLabel" if your enemy scene structure is different!
			var hpLabel = enemyNode.GetNode<Label>("StatusUI/HPLabel"); // Adjust path if needed!
			if (hpLabel != null)
			{
				hpLabel.Text = $"HP: {enemyData.Health:F0}";
				 // Maybe change color or show 'Defeated' if health <= 0
				hpLabel.Visible = enemyData.Health > 0;
			}

			 // Update visibility/appearance of the main enemy node if defeated
			 // Check if the node is a CanvasItem before accessing Visible
			if (enemyNode is CanvasItem canvasItem)
			{
				canvasItem.Visible = enemyData.Health > 0; // Set visibility on the CanvasItem
			}
			else
			{
				// Optional: Log a warning if the root isn't a CanvasItem.
				// GD.PrintW($"Enemy node root for {enemyData.Race} is not a CanvasItem, visibility cannot be set directly.");
			}
		}
	}

	/// <summary>
	/// Clears and repopulates the shop item list UI.
	/// </summary>
	private void UpdateShopUI()
	{
		// Clear previous items
		foreach (Node child in _shopItemList.GetChildren())
		{
			child.QueueFree(); // Remove and free old item entries
		}

		// TODO: Update coin display using actual hero.GetCoins()
		// _shopHeroCoinsLabel.Text = $"Your Coins: {MainHero.Instance.GetCoins()}";
		 _shopHeroCoinsLabel.Text = "Your Coins: ???"; // Placeholder

		// Populate with current shop items
		if (_shopItemEntryTemplate == null)
		{
			GD.PrintErr("Shop item template not loaded. Cannot populate shop UI.");
			return;
		}

		for (int i = 0; i < shopItems.Count; i++)
		{
			IItem item = shopItems[i];
			if (!itemPrices.TryGetValue(item.Name, out int price))
			{
				price = -1; // Indicate unknown price
			}

			// Instance the template scene
			Node itemEntryInstance = _shopItemEntryTemplate.Instantiate();

			// Get nodes within the instanced entry (adjust paths based on your template structure)
			var nameLabel = itemEntryInstance.GetNode<Label>("HBoxContainer/ItemDetails/ItemNameLabel");
			var descLabel = itemEntryInstance.GetNode<Label>("HBoxContainer/ItemDetails/ItemDescriptionLabel");
			var priceLabel = itemEntryInstance.GetNode<Label>("HBoxContainer/ItemPriceLabel");
			var buyButton = itemEntryInstance.GetNode<Button>("HBoxContainer/BuyButton");
			// var icon = itemEntryInstance.GetNode<TextureRect>("HBoxContainer/ItemIcon"); // If using icons

			// Populate the nodes
			nameLabel.Text = item.Name;
			descLabel.Text = item.Description;
			priceLabel.Text = (price >= 0) ? $"Price: {price}" : "Price: N/A";
			// icon.Texture = LoadItemIcon(item.Name); // TODO: Implement icon loading

			// Disable buy button if price is unknown or cannot afford (needs GetCoins)
			bool canAfford = false; // TODO: Replace with actual check: MainHero.Instance.GetCoins() >= price;
			buyButton.Disabled = price < 0; // || !canAfford; // Uncomment affordability check later

			// Connect the Buy button's pressed signal dynamically
			// Pass the item index 'i' using Bind so the handler knows which item was clicked
			buyButton.Pressed += () => _on_ShopItem_BuyButtonPressed(i); // Using lambda for simplicity here
			// Alternative: buyButton.Connect("pressed", new Callable(this, nameof(_on_ShopItem_BuyButtonPressed)).Bind(i));

			// Add the populated entry to the list
			_shopItemList.AddChild(itemEntryInstance);
		}
	}

	/// <summary>
	/// Creates visual representations (scenes) for the current logical enemies.
	/// </summary>
	private void CreateEnemyVisuals()
	{
		ClearEnemyVisuals(); // Ensure area is empty first

		float startX = 0; // Adjust starting position as needed
		float spacing = 200; // Adjust spacing between enemies

		for (int i = 0; i < currentEnemies.Count; i++)
		{
			IEnemy enemyData = currentEnemies[i];

			if (_enemyPrefabs.TryGetValue(enemyData.Race, out PackedScene prefab))
			{
				if (prefab != null)
				{
					Node enemyNode = prefab.Instantiate();
					_enemyArea.AddChild(enemyNode);
					// Position the enemy node
					if (enemyNode is Node2D enemyNode2D) // Set position if it's a Node2D
					{
						 enemyNode2D.Position = new Vector2(startX + (i * spacing), 0);
					}
					// Store the link between the scene node and the data
					_enemyNodes.Add(enemyNode, enemyData);
					// Optionally, pass the IEnemy data to a script on the enemy node if it needs it
					// if(enemyNode.HasMethod("SetEnemyData")) { enemyNode.Call("SetEnemyData", enemyData); }
				}
				else GD.PrintErr($"Prefab for {enemyData.Race} is null!");
			}
			else GD.PrintErr($"No prefab found for enemy race: {enemyData.Race}");
		}
		 UpdateCombatUI(); // Update HP displays for newly created enemies
	}

	 /// <summary>
	/// Removes all enemy nodes from the scene and clears the tracking dictionary.
	/// </summary>
	private void ClearEnemyVisuals()
	{
		foreach (Node enemyNode in _enemyArea.GetChildren())
		{
			enemyNode.QueueFree();
		}
		_enemyNodes.Clear();
	}


	#endregion

	#region Combat Logic & Turn Management

	public void StartHeroTurn()
	{
		if (currentState != GameState.Combat) return;
		GD.Print("Hero's Turn Begins.");
		_turnIndicatorLabel.Text = "Hero Turn";
		// Enable player action buttons
		_attackButton.Disabled = false;
		_endTurnButton.Disabled = false;
		// TODO: Add visual indication, highlight buttons, etc.
	}

	public void StartEnemyTurn()
	{
		if (currentState != GameState.Combat) return;
		GD.Print("Enemies' Turn Begins.");
		 _turnIndicatorLabel.Text = "Enemy Turn";
		// Disable player action buttons during enemy turn
		_attackButton.Disabled = true;
		_endTurnButton.Disabled = true;

		// --- Enemy Actions ---
		// Use a timer or sequence for clarity in a real game
		foreach (var kvp in _enemyNodes) // Iterate through VISIBLE enemies linked to data
		{
			IEnemy enemy = kvp.Value;
			Node enemyNode = kvp.Key;

			if (enemy.Health > 0) // Only living enemies act
			{
				GD.Print($"{enemy.Race} acts...");
				// Simple AI: Attack the hero
				double enemyAttackDamage = enemy.Attack();
				GD.Print($"{enemy.Race} attacks for {enemyAttackDamage:F1} potential damage.");

				// Apply damage to the hero
				var heroInstance = MainHero.Instance;
				double actualDamageTaken = heroInstance.TakeDamage((double)enemyAttackDamage);
				GD.Print($"Hero took {actualDamageTaken:F1} damage.");
				UpdateHeroStatusUI(); // Update HP display

				// --- Check for Hero Death ---
				// if (heroInstance.GetHealth() <= 0) // <<<< TODO: Need GetHealth()
				// {
				//     GameOver();
				//     return; // Stop processing turns if game is over
				// }
			}
		}

		// Check if combat ended AFTER all enemies acted (in case last enemy defeated hero)
		if (currentState == GameState.GameOver) return;

		// --- End Enemy Turn ---
		GD.Print("Enemies' Turn Finished.");
		CheckCombatEnd(); // Check if player won

		// If combat is still ongoing, start hero's turn again
		if (currentState == GameState.Combat)
		{
			StartHeroTurn();
		}
	}

	/// <summary>
	/// Logic for when the hero attacks an enemy.
	/// Needs target selection mechanism.
	/// </summary>
	public void HeroAttacksEnemy(IEnemy targetEnemy) // Pass the actual IEnemy data
	{
		if (currentState != GameState.Combat || targetEnemy == null || targetEnemy.Health <= 0)
		{
			GD.Print("Cannot attack: Invalid state or target.");
			return;
		}

		var heroInstance = MainHero.Instance;
		double heroAttackDamage = heroInstance.Atack();
		GD.Print($"Hero attacks {targetEnemy.Race} for {heroAttackDamage:F1} potential damage.");

		double actualDamageDealt = targetEnemy.TakeDamage(heroAttackDamage);
		GD.Print($"{targetEnemy.Race} took {actualDamageDealt:F1} damage. Remaining health: {targetEnemy.Health:F1}");

		// Update the specific enemy's HP display
		UpdateCombatUI(); // Update all enemy UI for simplicity, or target the specific node

		// Check if the enemy was defeated
		if (targetEnemy.Health <= 0)
		{
			GD.Print($"{targetEnemy.Race} defeated!");
			// Visuals are updated in UpdateCombatUI (hides node/label)
		}

		// Check if all enemies are defeated after the attack
		CheckCombatEnd();

		 // If combat is still ongoing, proceed to enemy turn
		 if(currentState == GameState.Combat)
		 {
			StartEnemyTurn();
		 }
	}

	 /// <summary>
	/// Checks if all enemies are defeated. If so, transitions to EndCombatPhase.
	/// </summary>
	private void CheckCombatEnd()
	{
		if (currentState != GameState.Combat) return;

		// Check if ANY enemy currently in the list still has health > 0
		bool anyEnemyAlive = currentEnemies.Any(enemy => enemy.Health > 0);

		if (!anyEnemyAlive && currentEnemies.Count > 0) // Check count > 0 to prevent winning on level 0 with no enemies
		{
			GD.Print("All enemies defeated!");
			EndCombatPhase();
		}
		// Else, combat continues...
	}

	#endregion

	#region Signal Handlers

	private void _on_AttackButton_pressed()
	{
		if (currentState != GameState.Combat) return;
		GD.Print("Attack Button Pressed");

		// --- Target Selection (Simple: First living enemy) ---
		IEnemy target = null;
		foreach (var enemy in currentEnemies)
		{
			if (enemy.Health > 0)
			{
				target = enemy;
				break; // Attack the first one found
			}
		}

		if (target != null)
		{
			 HeroAttacksEnemy(target);
		}
		else
		{
			GD.Print("No living enemies to attack!");
			// Should not happen if CheckCombatEnd works correctly, but good failsafe
			 CheckCombatEnd(); // Double-check if we should have already won
		}
		// In a real game: allow clicking on enemy node, find corresponding IEnemy from _enemyNodes
	}

	private void _on_EndTurnButton_pressed()
	{
		if (currentState != GameState.Combat) return;
		GD.Print("End Turn Button Pressed");
		StartEnemyTurn();
	}

	private void _on_LeaveShopButton_pressed()
	{
		if (currentState != GameState.Shop) return;
		GD.Print("Leave Shop Button Pressed");
		ExitShopPhase();
	}

	/// <summary>
	/// Handles the press of a Buy button on a dynamically created shop item entry.
	/// </summary>
	/// <param name="itemIndex">The index of the item in the 'shopItems' list.</param>
	private void _on_ShopItem_BuyButtonPressed(int itemIndex)
	{
		if (currentState != GameState.Shop) return;

		if (itemIndex < 0 || itemIndex >= shopItems.Count)
		{
			GD.PrintErr($"Invalid shop item index received from button signal: {itemIndex}");
			return;
		}

		IItem itemToBuy = shopItems[itemIndex];
		GD.Print($"Buy button pressed for item index {itemIndex}: {itemToBuy.Name}");

		// --- Actual Buy Logic ---
		if (!itemPrices.TryGetValue(itemToBuy.Name, out int price))
		{
			GD.PrintErr($"Cannot buy {itemToBuy.Name}: Price not found!");
			return;
		}

		var heroInstance = MainHero.Instance;

		// --- Real Coin Check (NEEDS GetCoins() and SpendCoins(int) or AddCoins(-) ---
		bool canAfford = false;
		int currentHeroCoins = 0; // = heroInstance.GetCoins(); // <<<< TODO: Need GetCoins()

		// ---- SIMULATION ---- Remove when hero has GetCoins() ----
		GD.Print("SIMULATION: Assuming hero can afford for now.");
		canAfford = true;
		// ---- END SIMULATION ----

		// Actual check would be:
		// if (currentHeroCoins >= price) { canAfford = true; }

		if (canAfford)
		{
			// --- Spend Coins ---
			// bool spent = heroInstance.SpendCoins(price); // <<<< TODO: Ideal method
			// if(!spent) { GD.PrintErr("Spending coins failed!"); return; }

			// Using AddCoins(-) as alternative
			heroInstance.AddCoins(-price); // <<<< ASSUMES AddCoins handles spending

			// --- Add Item ---
			heroInstance.AddItem((Item)itemToBuy.Copy()); // Cast IItem to Item
			GD.Print($"Purchased {itemToBuy.Name}!");

			// --- Update UI ---
			UpdateHeroStatusUI(); // Update coin display potentially
			UpdateShopUI(); // Refresh shop list (updates coin display, potentially disables button)
		}
		else
		{
			GD.Print($"Cannot afford {itemToBuy.Name}. Need {price} coins."); // , have {currentHeroCoins}."); // Add coin count when available
		}
	}

	#endregion

	// TODO: Implement methods in MainHero/IHero for GetHealth, GetMaxHealth, GetArmor, GetCoins, GetLevel, GetExperience, GetMaxExperience, SpendCoins
}
