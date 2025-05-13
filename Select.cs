using Godot;
using System;

public partial class Select : Control
{
	public override void _Ready()
	{
		// Connect button signals to set the hero and change scene
		GetNode<Button>("SelectScreen/CenterContainer/VBoxContainer/ArcherButton").Pressed += () => OnButtonPressed("Archer");
		GetNode<Button>("SelectScreen/CenterContainer/VBoxContainer/SwordsmanButton").Pressed += () => OnButtonPressed("Swordsman");
		GetNode<Button>("SelectScreen/CenterContainer/VBoxContainer/PirateButton").Pressed += () => OnButtonPressed("Pirate");
	}

		
	private void OnButtonPressed(string hero)
	{
		var global = GetNode<Global>("/root/Global");
		global.SelectedHero = hero;
		GD.Print($"Selected Hero: {global.SelectedHero}");

		GetTree().ChangeSceneToFile("res://Main.tscn");
	}
}
