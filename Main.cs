using Godot;

public partial class Main : Node2D
{
	public override void _Ready()
	{
		// Get the selected hero from the global singleton
		var global = GetNode<Global>("/root/Global");
		var label = GetNode<Label>("HeroLabel");
		label.Text = $"Selected Hero: {global.SelectedHero}";

		// Add your game logic here
	}
}
