using System.Collections.Generic;
using Godot;
using GodotPlugins.Game;
using MAPZ_lab_RPG.Entities.Heroes;
using Scenes.Managers;

public partial class StartMenu : Control
{
	private Button _archerButton;
	private Button _swordsmanButton;
	private Button _pirateButton;

	private const string BattleScenePath = "res://Scenes/BattleScene.tscn";

	private List<string> heroNames = new List<string>
	{
		"Default Hero 1",
		"Default Hero 2",
		"Default Hero 3",
		"Default Hero 4",
		"Default Hero 5",
		"Default Hero 6",
		"Default Hero 7",
	};

	public override void _Ready()
	{
		GD.Print("StartMenu ready!");
		heroNames = MainHero.Instance.GetHeroNames(); // Ensure hero names are loaded
		var heroButtonContainer = GetNode<Control>("CenterContainer/VBoxContainer/ScrollContainer/HBoxContainer");
		foreach (var heroName in heroNames)
		{
			GD.Print($"Available hero: {heroName}");
			var buttonControl = GD.Load<PackedScene>($"res://Scenes/HeroSelectButton.tscn").Instantiate<CenterContainer>();
			var button = buttonControl.GetNode<Button>("Button");
			button.Text = heroName;
			button.Pressed += () => OnHeroSelected(heroName);
			heroButtonContainer.AddChild(buttonControl);
		}
	}

	private void OnHeroSelected(string heroName)
	{
		GD.Print($"Hero selected: {heroName}");

		GameData.SelectedHeroName = heroName;
		MainHero.Instance.HeroSelect(heroName);

		// Change to the battle scene
		var error = GetTree().ChangeSceneToFile(BattleScenePath);
		if (error != Error.Ok)
		{
			GD.PrintErr($"Error changing scene to {BattleScenePath}: {error}");
		}
	}
}
