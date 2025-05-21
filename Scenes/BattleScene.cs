// BattleScene.cs
using Godot;
using Scenes.Managers;

public partial class BattleScene : Node2D // Or Control, or whatever your root node type is
{
	private Label _heroDisplayLabel;
	private Node2D _heroDisplayNode;
	
	private MainHeroManager _mainHeroManager;

	public override void _Ready()
	{
		// GD.Print("BattleScene ready!");
		// _heroDisplayLabel = GetNode<Label>("HDL");
		// GD.Print($"Label found! {_heroDisplayLabel.Name}");

		string selectedHero = GameData.SelectedHeroName;
		// GD.Print($"Selected hero: {selectedHero.ToString()}");
		// _heroDisplayLabel.Text = $"Selected Hero: {selectedHero}";

		_heroDisplayNode = GD.Load<PackedScene>("res://Scenes/Entity.tscn").Instantiate<Node2D>();
		GetNode<CenterContainer>("Control/VBoxContainer/HBoxContainer2/CenterContainer").AddChild(_heroDisplayNode);
		AddChild(_heroDisplayNode);
		_heroDisplayNode.Name = "HeroDisplayNode"; // Set a name for the node

		var MoneyLabel = GetNode<Label>("Control/VBoxContainer/HBoxContainer3/MoneyLabel");
		var LevelLabel = GetNode<Label>("Control/VBoxContainer/HBoxContainer3/LevelLabel");

		_mainHeroManager = new MainHeroManager(_heroDisplayNode, selectedHero, MoneyLabel, LevelLabel);
	}

}
