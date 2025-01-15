using System;

using Godot;

namespace ZenvaHexMap.Game;

public partial class CityUI : Panel
{
  private City _city;
  Label _cityNameLabel, _cityPopLabel, _cityFoodLabel, _cityProdLabel;

  public City City
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
        _city.PropertyChanged += City_PropertyChanged;

      City_PropertyChanged(_city, null);
    }
  }

  private void City_PropertyChanged(object sender, EntityUpdatedEventArgs<City> e)
  {
    _cityNameLabel.Text = _city.CityName;
    _cityPopLabel.Text  = $"Population : {_city.Population}";
    _cityFoodLabel.Text = $"Food : {_city.TotalFood}";
    _cityProdLabel.Text = $"Production : {_city.TotalProduction}";
  }

  public override void _Ready()
  {
    _cityNameLabel = GetNode<Label>("CityName");
    _cityPopLabel  = GetNode<Label>("Population");
    _cityFoodLabel = GetNode<Label>("Food");
    _cityProdLabel = GetNode<Label>("Production");
  }
}
