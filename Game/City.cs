using Godot;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ZenvaHexMap.Game;

public partial class City : Node2D, INotifyEntityPropertyChanged<EntityUpdatedEventArgs<City>>
{
  public static int PopulationThresholdIncrease {get;} = 15;

  public HexTileMap Map { get; set; }
  public Vector2I CityCentreCoordinates { get; set; }

  public List<Hex> CityTerritory { get; } = [];
  public List<Hex> BorderTilePool
    => CityTerritory.SelectMany(h => Map.GetAdjacentHexes(h.Coordinates).Where(h => IsValidNeightbourTile(h)))
                    .Distinct().ToList();

  public int Population { get; set; } = 1;
  private int PopulationGrowthThreshold { get; set; }
  private int PopulationGrowthTracker { get; set; }
  public int TotalFood => CityTerritory.Sum(t => t.Food);
  public int TotalProduction => CityTerritory.Sum(t => t.Production);

  public List<Unit> UnitBuildQueue {get;} = [];
  public Unit? UnitBeingBuilt => UnitBuildQueue.FirstOrDefault();
  public int UnitProductionProgress {get;private set;}

  public Civilization OwnerCivilization
  {
    get => _ownerCivilization;
    set
    {
      _ownerCivilization = value;

      _sprite ??= GetNode<Sprite2D>("Sprite2D");
      _sprite.Modulate = _ownerCivilization.CivilizationTerritoryColor;
    }
  }

  public void AddUnitToBuildQueue(Unit unit)
  {
    UnitBuildQueue.Add(unit);
    PropertyChanged?.Invoke(this, new EntityUpdatedEventArgs<City?>(this));
  }

  public string CityName
  {
    get => _cityName;
    set
    {
      _cityName = value;
      _label ??= GetNode<Label>("Label");
      _label.Text = _cityName;
    }
  }

  public void ProcessTurn()
  {
    PopulationGrowthTracker += TotalFood;

    if (PopulationGrowthTracker > PopulationGrowthThreshold)
    {
      Population++;
      PopulationGrowthTracker = 0;
      PopulationGrowthThreshold += PopulationThresholdIncrease;

      AddRandomNewTile();
      Map.UpdateCivTerritoryMap(_ownerCivilization);

      PropertyChanged?.Invoke(this, new EntityUpdatedEventArgs<City>(this));
    }
  }

  private Sprite2D _sprite;
  private Label _label;
  private Civilization _ownerCivilization;
  private string _cityName;

  public event EventHandler<EntityUpdatedEventArgs<City?>> PropertyChanged;

  public void AddTerritory(IEnumerable<Hex> territoryToAdd)
  {
    foreach (var hex in territoryToAdd)
    {
      hex.OwnerCity = this;
      CityTerritory.Add(hex);
    }
  }

  private void AddRandomNewTile()
  {
    if (BorderTilePool.Count > 0)
    {
      var rand = new Random();
      var index = rand.Next(BorderTilePool.Count);

      AddTerritory([BorderTilePool[index]]);
      BorderTilePool.RemoveAt(index);
    }
  }

  private static bool IsValidNeightbourTile(Hex h)
    => h.OwnerCity is null && (h.TerrainType is not TerrainTypes.Water and not TerrainTypes.Ice and not TerrainTypes.Mountain);
}
