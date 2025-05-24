using Godot;
using MAPZ_lab_RPG.Entities.Heroes;
using Scenes.Managers;

public partial class GameData : Node
{
	public GameData() { }

	public static string SelectedHeroName { get; set; }
	public static int CurrentRound { get; set; } = 1;
	public static MainHeroManager MainHeroManager { get; set;} = null;
}
