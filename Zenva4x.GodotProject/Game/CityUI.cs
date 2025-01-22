using System;

using Godot;

namespace Zenva4x.GodotProject.Game;

public partial class CityUI : Panel
{
  private City? _city;
  Label? _cityNameLabel, _cityPopLabel, _cityFoodLabel, _cityProdLabel;
  VBoxContainer? _buildMenuContainer;

  public City? City
  {
    get
    {
      return _city;
    }
    set
    {
      if (_city is not null)
        _city.PropertyChanged -= City_PropertyChanged;

      _city = value;

      if (_city is not null)
      {
        _city.PropertyChanged += City_PropertyChanged;
        
        PopulateCityBuildQueueUI(_city);
        ConnectUnitBuildSignals(_city);
        City_PropertyChanged(_city, new EntityUpdatedEventArgs<City?>(_city));
      }
    }
  }

  private void PopulateCityBuildQueueUI(City city)
  {
    var queue = GetNode<VBoxContainer>("BuildQueueContainer/VBoxContainer")
              ?? throw new InvalidOperationException("Unable to find node BuildQueueContainer/VBoxContainer");
    
    foreach (var node in queue.GetChildren())
    {
      queue.RemoveChild(node);
      node.QueueFree();
    }

    for (var i = 0; i < city.UnitBuildQueue.Count; i++)
    {
      if (i == 0)
      {
        var unit = city.UnitBeingBuilt;

        if (unit is not null)
        {
          queue.AddChild(new Label{
            Text = $"{unit.UnitName} {city.UnitProductionProgress}/{unit.ProductionRequired}"
          });
        }
      }
      else
      {
        var unit = city.UnitBuildQueue[i];

        if (unit is not null)
        {
          queue.AddChild(new Label{
            Text = $"{unit.UnitName} 0/{unit.ProductionRequired}"
          });
        }
      }
    }
  }  

  private void City_PropertyChanged(object? sender, EntityUpdatedEventArgs<City?> e)
  {
    if (_city is null) return;

    _cityNameLabel.Text = _city.CityName;
    _cityPopLabel.Text  = $"Population : {_city.Population}";
    _cityFoodLabel.Text = $"Food : {_city.TotalFood}";
    _cityProdLabel.Text = $"Production : {_city.TotalProduction}";

    PopulateCityBuildQueueUI(_city);
  }

  public void ConnectUnitBuildSignals(City city)
  {
    _buildMenuContainer
      ??= GetNode<VBoxContainer>("BuildMenuContainer/VBoxContainer") 
      ?? throw new InvalidOperationException("Unable to find node BuildMenuContainer/VBoxContainer");

    var settlerButton = _buildMenuContainer.GetNode<UnitBuildButton>("SettlerBuildButton");
    settlerButton.Unit = new Settler();

    var warriorButton = _buildMenuContainer.GetNode<UnitBuildButton>("WarriorBuildButton");
    warriorButton.Unit = new Warrior();

    settlerButton.OnPressed += city.AddUnitToBuildQueue;
    settlerButton.OnPressed += _ => City_PropertyChanged(this, new EntityUpdatedEventArgs<City?>(city));
    warriorButton.OnPressed += city.AddUnitToBuildQueue;
    warriorButton.OnPressed += _ => City_PropertyChanged(this, new EntityUpdatedEventArgs<City?>(city));
  }

  public override void _Ready()
  {
    _cityNameLabel = GetNode<Label>("CityName");
    _cityPopLabel  = GetNode<Label>("Population");
    _cityFoodLabel = GetNode<Label>("Food");
    _cityProdLabel = GetNode<Label>("Production");
  }
}
