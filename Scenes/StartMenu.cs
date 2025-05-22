// StartMenu.cs
using Godot;
using MAPZ_lab_RPG.Entities.Heroes;

public partial class StartMenu : Control
{
	private Button _archerButton;
	private Button _swordsmanButton;
	private Button _pirateButton;

	// Path to your battle scene
	private const string BattleScenePath = "res://Scenes/BattleScene.tscn"; // IMPORTANT: Update this path!

	public override void _Ready()
	{
		GD.Print("StartMenu ready!");
		// Get references to the buttons. Adjust paths if your scene tree is different.
		_archerButton = GetNode<Button>("CenterContainer/VBoxContainer/ArcherButton");
		_swordsmanButton = GetNode<Button>("CenterContainer/VBoxContainer/SwordsmanButton");
		_pirateButton = GetNode<Button>("CenterContainer/VBoxContainer/PirateButton");

		// Connect the 'pressed' signal of each button to a method.
		// We use lambda expressions for conciseness here.
		_archerButton.Pressed += () => OnHeroSelected(HeroEnum.ARCHER);
		_swordsmanButton.Pressed += () => OnHeroSelected(HeroEnum.SWORDSMAN);
		_pirateButton.Pressed += () => OnHeroSelected(HeroEnum.PIRATE);
		GD.Print("Binded buttons!");

	}

	private void OnHeroSelected(HeroEnum heroName)
	{
		GD.Print($"Hero selected: {heroName}");

		// Store the selected hero name in our Autoload/Singleton
		GameData.SelectedHeroName = heroName;

		// Change to the battle scene
		var error = GetTree().ChangeSceneToFile(BattleScenePath);
		if (error != Error.Ok)
		{
			GD.PrintErr($"Error changing scene to {BattleScenePath}: {error}");
		}
	}
}
