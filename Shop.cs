using Godot;
using MAPZ_lab_RPG.Entities.Heroes;
using MAPZ_lab_RPG.Entities.Items;
using System.Collections.Generic;

public partial class Shop : Control
{
    [Signal]
    public delegate void ShopClosedEventHandler();

    private MainHero _mainHero;
    private ItemPrototypeManager _itemManager;
    private Label _coinLabel;
    private VBoxContainer _itemContainer;
    private Dictionary<string, int> _itemPrices;

    public override void _Ready()
    {
        _mainHero = MainHero.Instance;
        _itemManager = ItemPrototypeManager.Instance;

        // Initialize item prices
        _itemPrices = new Dictionary<string, int>
        {
            { "HealthRune", 100 },
            { "DamageRune", 150 },
            { "ArmorRune", 125 }
        };

        // Set up UI
        _coinLabel = new Label();
        _coinLabel.Position = new Vector2(10, 10);
        AddChild(_coinLabel);

        _itemContainer = new VBoxContainer();
        _itemContainer.Position = new Vector2(10, 50);
        AddChild(_itemContainer);

        // Add items to shop
        AddShopItem("HealthRune");
        AddShopItem("DamageRune");
        AddShopItem("ArmorRune");

        // Add exit button
        var exitButton = new Button();
        exitButton.Text = "Exit Shop";
        exitButton.Position = new Vector2(10, 200);
        exitButton.Pressed += OnExitPressed; // Use direct delegate assignment
        AddChild(exitButton);

        UpdateCoinDisplay();
    }

    private void AddShopItem(string prototypeName)
    {
        var item = _itemManager.CreateItem(prototypeName);
        int price = _itemPrices[prototypeName];

        var hbox = new HBoxContainer();
        var label = new Label();
        label.Text = $"{item.Name}: {item.Description} (Health: {item.Health}, Damage: {item.Damage}, Armor: {item.Armor}) - {price} coins";
        hbox.AddChild(label);

        var buyButton = new Button();
        buyButton.Text = "Buy";
        buyButton.Pressed += () => OnBuyPressed(prototypeName, price); // Use lambda directly with Pressed event
        hbox.AddChild(buyButton);

        // Disable button if player can't afford the item
        int currentCoins = _mainHero.AddCoins(0);
        if (currentCoins < price)
        {
            buyButton.Disabled = true;
            buyButton.Modulate = Colors.Gray; // Visually indicate the button is disabled
        }

        _itemContainer.AddChild(hbox);
    }

    private void OnBuyPressed(string prototypeName, int price)
    {
        int currentCoins = _mainHero.AddCoins(0);
        if (currentCoins >= price)
        {
            // Deduct coins
            _mainHero.AddCoins(-price);

            // Add item to hero (cast IItem to Item since MainHero.AddItem expects Item)
            var item = _itemManager.CreateItem(prototypeName) as Item;
            if (item != null)
            {
                _mainHero.AddItem(item);
                GD.Print($"Purchased {item.Name} for {price} coins!");
            }

            UpdateCoinDisplay();

            // Update all buy buttons' states
            foreach (Node child in _itemContainer.GetChildren())
            {
                if (child is HBoxContainer hbox)
                {
                    var button = hbox.GetChild<Button>(1);
                    var itemPriceText = hbox.GetChild<Label>(0).Text.Split(" - ")[1].Replace(" coins", "");
                    if (int.TryParse(itemPriceText, out int itemPrice))
                    {
                        int newCoinBalance = _mainHero.AddCoins(0);
                        button.Disabled = newCoinBalance < itemPrice;
                        button.Modulate = button.Disabled ? Colors.Gray : Colors.White;
                    }
                }
            }
        }
        else
        {
            GD.Print("Not enough coins!");
        }
    }

    private void OnExitPressed()
    {
        EmitSignal(SignalName.ShopClosed);
        QueueFree();
    }

    private void UpdateCoinDisplay()
    {
        int coins = _mainHero.AddCoins(0);
        _coinLabel.Text = $"Coins: {coins}";
    }
}