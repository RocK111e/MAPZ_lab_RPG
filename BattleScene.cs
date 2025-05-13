// BattleScene.cs
using Godot;

public partial class BattleScene : Node2D // Or Control, or whatever your root node type is
{
	private Label _heroDisplayLabel;

	public override void _Ready()
	{
		// Get reference to the label. Adjust path if needed.
		_heroDisplayLabel = GetNode<Label>("HeroDisplayLabel");

		// Retrieve the selected hero name from our Autoload/Singleton
		string selectedHero = GameData.SelectedHeroName;

		if (string.IsNullOrEmpty(selectedHero))
		{
			_heroDisplayLabel.Text = "No hero was selected!";
			GD.PrintErr("BattleScene loaded, but no hero name found in GameData.");
		}
		else
		{
			_heroDisplayLabel.Text = $"Fighting as: {selectedHero}";
			GD.Print($"BattleScene loaded with hero: {selectedHero}");
		}

		// You can now use 'selectedHero' to customize your battle scene
		// e.g., load different character sprites, abilities, etc.
	}
}
