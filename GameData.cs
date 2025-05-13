using Godot;

public partial class GameData : Node
{
    // This property will hold the selected hero's name.
    // 'static' makes it accessible globally via the class name.
    public static string SelectedHeroName { get; set; }

    // Godot requires a parameterless constructor for nodes that are instanced.
    // While not strictly necessary for an Autoload that's only a static container,
    // it's good practice.
    public GameData() {}
}