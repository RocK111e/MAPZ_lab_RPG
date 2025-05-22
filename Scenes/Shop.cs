using Godot;
using System;

public partial class Shop : Node2D
{
	public override void _Ready(){
		GD.Print("Shop scene loaded.");
		GetTree().ChangeSceneToFile("res://Scenes/BattleScene.tscn");
	}
}
