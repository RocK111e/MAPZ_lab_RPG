using Godot;

class MainHeroManager
{
    private Node2D _heroDisplayNode;

    private Label _heroNameLabel;
    private string _heroName;
    private ProgressBar _heroHealthBar;
    private Label _heroHealthLabel;

    public MainHeroManager(Node2D heroDisplayNode, string herName)
    {
        _heroDisplayNode = heroDisplayNode;
        _heroName = herName;
        _heroNameLabel = heroDisplayNode.GetNode<Label>("HeroNameLabel");
        _heroNameLabel.Text = _heroName;

        _heroHealthBar = heroDisplayNode.GetNode<ProgressBar>("HeroHealthBar");
        _heroHealthLabel = heroDisplayNode.GetNode<Label>("HeroHealthLabel");
    }
    public void ApplyDamage(int damage)
    {
        
    }
    
}