// SelectScreen.cs
using Godot;

public partial class SelectScreen : Control
{
	public override void _Ready()
	{
		GD.Print("SelectScreen _Ready() CALLED."); // <-- ADD THIS LINE

		Button archerButton = GetNode<Button>("CenterContainer/VBoxContainer/ArcherButton");
		if (archerButton == null)
		{
			GD.PrintErr("ArcherButton NOT FOUND at path: CenterContainer/VBoxContainer/ArcherButton");
		}
		else
		{
			GD.Print("ArcherButton found. Connecting Pressed signal.");
			archerButton.Pressed += () => OnButtonPressed("Archer");
		}

		Button swordsmanButton = GetNode<Button>("CenterContainer/VBoxContainer/SwordsmanButton");
		if (swordsmanButton == null)
		{
			GD.PrintErr("SwordsmanButton NOT FOUND at path: CenterContainer/VBoxContainer/SwordsmanButton");
		}
		else
		{
			GD.Print("SwordsmanButton found. Connecting Pressed signal.");
			swordsmanButton.Pressed += () => OnButtonPressed("Swordsman");
		}

		Button pirateButton = GetNode<Button>("CenterContainer/VBoxContainer/PirateButton");
		if (pirateButton == null)
		{
			GD.PrintErr("PirateButton NOT FOUND at path: CenterContainer/VBoxContainer/PirateButton");
		}
		else
		{
			GD.Print("PirateButton found. Connecting Pressed signal.");
			pirateButton.Pressed += () => OnButtonPressed("Pirate");
		}
	}

	private void OnButtonPressed(string hero)
	{
		GD.Print($"OnButtonPressed CALLED with hero: {hero}"); // <-- ADD THIS LINE

		var global = GetNode<Global>("/root/Global");
		if (global == null)
		{
			GD.PrintErr("Global singleton not found! Make sure it's autoloaded.");
			return;
		}
		global.SelectedHero = hero;
		GD.Print($"Selected Hero stored in Global: {global.SelectedHero}");

		Error err = GetTree().ChangeSceneToFile("res://Main.tscn");
		if (err != Error.Ok)
		{
			GD.PrintErr($"Error changing scene to Main.tscn: {err}");
		}
		else
		{
			GD.Print("Successfully initiated scene change to Main.tscn");
		}
	}
}
