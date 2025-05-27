using Godot;
using Scenes.Managers;

public partial class GameData : Node
{
	public GameData() { }

	public static int CurrentRound { get; set; } = 1;
	public static GameManagerFacade GameManagerFacade { get; set; } = null;
}
