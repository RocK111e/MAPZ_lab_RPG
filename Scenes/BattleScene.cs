// BattleScene.cs
using Godot;

public partial class BattleScene : Node2D // Or Control, or whatever your root node type is
{
	private Label _heroDisplayLabel;

	public override void _Ready()
	{
		GD.Print("BattleScene ready!");
		// Get reference to the label. Adjust path if needed.
		_heroDisplayLabel = GetNode<Label>("HDL");
		GD.Print($"Label found! {_heroDisplayLabel.Name}");
		// Retrieve the selected hero name from our Autoload/Singleton
		string selectedHero = GameData.SelectedHeroName;
		GD.Print($"Selected hero: {selectedHero.ToString()}");
		_heroDisplayLabel.Text = $"Selected Hero: {selectedHero}";
	}
}
