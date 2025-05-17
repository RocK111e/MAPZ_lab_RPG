using Godot;
using MAPZ_lab_RPG.Entities.Heroes;

namespace Scenes.Managers;
class MainHeroManager
{
	private Node2D _heroDisplayNode;
    private MainHero _hero;

	private Label _heroNameLabel;
	private string _heroName;
	private ProgressBar _heroHealthBar;
	private Label _heroHealthLabel;

    public MainHeroManager(Node2D heroDisplayNode, string herName)
    {
        _heroDisplayNode = heroDisplayNode;
        _heroName = herName;
        _heroNameLabel = heroDisplayNode.GetNode<Label>("Label");
        _heroNameLabel.Text = _heroName;

        _heroHealthBar = heroDisplayNode.GetNode<ProgressBar>("ProgressBar");
        _heroHealthLabel = heroDisplayNode.GetNode<Label>("ProgressBar/HPLabel");
        
        _hero = MainHero.Instance;
	}
	public void ApplyDamage(int damage)
	{
		
	}
	
}
