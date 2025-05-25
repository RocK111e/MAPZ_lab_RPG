using System.Collections.Generic;
using Godot;
using MAPZ_lab_RPG.Entities.Heroes;

public partial class StartMenu : Control
{
	private Button _archerButton;
	private Button _swordsmanButton;
	private Button _pirateButton;

	private const string BattleScenePath = "res://Scenes/BattleScene.tscn";

	private List<string> heroNames = new List<string>
	{
		"Archer",
		"Swordsman",
		"Pirate",
		"Wizard",
		"Rogue",
		"Paladin",
		"Barbarian",
	};

	public override void _Ready()
	{
		GD.Print("StartMenu ready!");
		var heroButtonContainer = GetNode<Control>("CenterContainer/VBoxContainer/ScrollContainer/HBoxContainer");
		foreach (var heroName in heroNames)
		{
			GD.Print($"Available hero: {heroName}");
			var buttonControl = GD.Load<PackedScene>($"res://Scenes/HeroSelectButton.tscn").Instantiate<CenterContainer>();
			var button = buttonControl.GetNode<Button>("Button");
			button.Text = heroName;
			//heroButton.Pressed += () => OnHeroSelected(heroName);
			heroButtonContainer.AddChild(buttonControl);
		}

		// _archerButton = GetNode<Button>("CenterContainer/VBoxContainer/ArcherButton");
		// _swordsmanButton = GetNode<Button>("CenterContainer/VBoxContainer/SwordsmanButton");
		// _pirateButton = GetNode<Button>("CenterContainer/VBoxContainer/PirateButton");


		// _archerButton.Pressed += () => OnHeroSelected(HeroEnum.ARCHER);
		// _swordsmanButton.Pressed += () => OnHeroSelected(HeroEnum.SWORDSMAN);
		// _pirateButton.Pressed += () => OnHeroSelected(HeroEnum.PIRATE);
		// GD.Print("Binded buttons!");

	}

	// private void OnHeroSelected(HeroEnum heroName)
	// {
	// 	GD.Print($"Hero selected: {heroName}");

	// 	GameData.SelectedHeroName = heroName;

	// 	// Change to the battle scene
	// 	var error = GetTree().ChangeSceneToFile(BattleScenePath);
	// 	if (error != Error.Ok)
	// 	{
	// 		GD.PrintErr($"Error changing scene to {BattleScenePath}: {error}");
	// 	}
	// }
}
