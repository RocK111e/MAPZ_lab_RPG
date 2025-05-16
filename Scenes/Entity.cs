using Godot;

public partial class Entitry : Node2D
{
    public Label _nameLabel;
    public Label _healthLabel;
    public Sprite2D _sprite;
    public ProgressBar _healthBar;

    public override void _Ready()
    {
        _nameLabel = GetNode<Label>("NameLabel");
        _healthLabel = GetNode<Label>("HealthLabel");
        _sprite = GetNode<Sprite2D>("Sprite");
    }

    void set_name(string name)
    {
        _nameLabel.Text = name;
    }

    void set_health(int health, int maxHealth)
    {
        _healthLabel.Text = $"{health}/{maxHealth}";
        _healthBar.Value = health;
        _healthBar.MaxValue = maxHealth;
    }
    

}