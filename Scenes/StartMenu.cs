using Godot;
using MAPZ_lab_RPG.Entities.Heroes;
using Scenes.Managers;

public partial class StartMenu : Control
{
	private Button _archerButton;
	private Button _swordsmanButton;
	private Button _pirateButton;

	private const string BattleScenePath = "res://Scenes/BattleScene.tscn";

	public override void _Ready()
	{
		GD.Print("StartMenu ready!");
		
		_archerButton = GetNode<Button>("CenterContainer/VBoxContainer/ArcherButton");
		_swordsmanButton = GetNode<Button>("CenterContainer/VBoxContainer/SwordsmanButton");
		_pirateButton = GetNode<Button>("CenterContainer/VBoxContainer/PirateButton");


		_archerButton.Pressed += () => OnHeroSelected("Archer");
		_swordsmanButton.Pressed += () => OnHeroSelected("Swordsman");
		_pirateButton.Pressed += () => OnHeroSelected("Pirate");
		GD.Print("Binded buttons!");

	}

	private void OnHeroSelected(string heroName)
	{
		GD.Print($"Hero selected: {heroName}");

		MainHero.Instance.HeroSelect(heroName);

		GameData.SelectedHeroName = heroName;

		var error = GetTree().ChangeSceneToFile(BattleScenePath);
		if (error != Error.Ok)
		{
			GD.PrintErr($"Error changing scene to {BattleScenePath}: {error}");
		}
	}
}
