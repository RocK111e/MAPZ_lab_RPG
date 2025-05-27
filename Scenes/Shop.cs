using Godot;

public partial class Shop : Node2D
{
	public override void _Ready()
	{
		GD.Print("Shop scene loaded.");
		
		GetNode<Button>("Button").Pressed += () =>
		{
			GD.Print("Continue button pressed.");
			GetTree().ChangeSceneToFile("res://Scenes/BattleScene.tscn");
		};
	}
}
