using System.Collections.Generic;

using Godot;

namespace Zenva4x.Core.Game.Entities.MapEntities;

public enum TerrainType
{
  Water,
  Plains,
  Desert,
  Mountain,
  Forest,
  Beach,
  ShallowWater,
  Ice,
  CivColorBase,
}

public class MapEntity : GameEntity
{
  public required int Width { get;init; }
  public required int Height { get;init; }
  public required int NumberOfAIPlayers { get;init; }
  public required Color PlayerCivilizationColor { get;init; }
  internal Dictionary<Vector2I, MapHexEntity> MapData { get;init;} = [];

  public static MapEntity Create(int width, int height)
  {
    var map = new MapEntity()
    {
      Width = width,
      Height = height,
      NumberOfAIPlayers = 0,
      PlayerCivilizationColor = new Color(255, 255, 255),
    };

    
    for (var x = 0; x < width; x++)
    {
      for (var y = 0; y < height; y++)
      {
        map.MapData[new Vector2I(x, y)] = new () {
          Coordinates = new Vector2I(x, y),
          TerrainType = TerrainType.Water,
          Food = 0,
          Production = 0
        };
      }
    }

    return map;
  }
}