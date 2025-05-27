using Godot;
using Scenes.Managers;

public partial class BattleScene : Node2D
{
	private Control _heroDisplayNode; 
	private GridContainer _enemyPlaceholderNode;
	private GameManagerFacade _GameManagerFacade; 

	private Label _moneyLabel;
	private Label _levelLabel;

	public override void _Ready()
	{
		// Round serup
		var RoundLabel = GetNode<Label>("Control/VBoxContainer/HBoxContainer/RoundLabel");
		RoundLabel.Text = $"Round: {GameData.CurrentRound}";

		// Hero setup
		var heroContainer = GetNode<CenterContainer>("Control/VBoxContainer/HBoxContainer2/CenterContainer");
		_moneyLabel = GetNode<Label>("Control/VBoxContainer/HBoxContainer3/MoneyLabel");
		_levelLabel = GetNode<Label>("Control/VBoxContainer/HBoxContainer3/LevelLabel");

		// Enemy setup
		_enemyPlaceholderNode = GetNode<GridContainer>("Control/VBoxContainer/HBoxContainer2/GridContainer");


		_GameManagerFacade = GameData.GameManagerFacade ?? new GameManagerFacade();
		GameData.GameManagerFacade = _GameManagerFacade;
		_GameManagerFacade.SetNodes(heroContainer, _enemyPlaceholderNode, _moneyLabel, _levelLabel, GameData.CurrentRound);

		_GameManagerFacade.EndBattle += SwitchToShopScene;
		_GameManagerFacade.LoseBattle += SwitchToLoseBattleScene;
	}

	private void SwitchToShopScene()
	{
		GD.Print("Switching to Shop scene.");
		GetTree().ChangeSceneToFile("res://Scenes/Shop.tscn");
	}

	public void SwitchToLoseBattleScene()
	{
		GD.Print("Switching to Lose Battle scene.");
		GetTree().ChangeSceneToFile("res://Scenes/LoseScreen.tscn");
	}

	public override void _ExitTree()
	{
		_GameManagerFacade.EndBattle -= SwitchToShopScene;
		_GameManagerFacade.LoseBattle -= SwitchToLoseBattleScene;
		_GameManagerFacade.Cleanup();
	}
}
