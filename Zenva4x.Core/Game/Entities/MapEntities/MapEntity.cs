using Godot;

using System.Linq;
using System.Runtime.CompilerServices;

using Zenva4x.Core.Wrappers.GodotWrappers;

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
  internal MapEntity() {}

  public required int Width { get;init; }
  public required int Height { get;init; }
  public required int NumberOfAIPlayers { get;init; }
  public required Color PlayerCivilizationColor { get;init; }
  internal Dictionary<Vector2I, MapHexEntity> MapData { get;init;} = [];

  public record TerrainRange(float Min, float Max, TerrainType TerrainType);

  private float NoiseMapMax { get; set; }

  public static MapEntity Create(IFastNoiseLite fastNoiseLite, int width, int height)
  {
    return Create(fastNoiseLite, width, height, [
      new (0    , .25f, TerrainType.Water),
      new (.25f , .50f, TerrainType.ShallowWater),
      new (.50f , .75f, TerrainType.Beach),
      new (.75f , 1.0f, TerrainType.Plains),
    ]);
  }

  public static MapEntity Create(IFastNoiseLite fastNoiseLite, int width, int height, IEnumerable<TerrainRange> terrainRanges)
  {
    var map = new MapEntity()
    {
      Width = width,
      Height = height,
      NumberOfAIPlayers = 0,
      PlayerCivilizationColor = new Color(255, 255, 255),
    };

    var baseNoiseMap = MakeSomeNoise(map, fastNoiseLite);

    DrawTerrain(map, baseNoiseMap, terrainRanges);

    return map;
  }

  private static float[,] MakeSomeNoise(MapEntity map, IFastNoiseLite fastNoiseLite)
  {
    var noiseMap = new float[map.Width, map.Height];

    for (var x = 0; x < map.Width; x++)
    {
      for (var y = 0; y < map.Height; y++)
      {
        noiseMap[x, y] = Math.Abs(fastNoiseLite.GetNoise2D(x, y));
      }
    }

    return noiseMap;
  }

  private static void DrawTerrain(MapEntity map, float[,] noiseMap, IEnumerable<TerrainRange> terrainRanges)
  {
    var noiseMax = noiseMap.Cast<float>().Max();

    for (var x = 0; x < map.Width; x++)
    {
      for (var y = 0; y < map.Height; y++)
      {
        var hex = new MapHexEntity{
          Coordinates = new Vector2I(x, y),
          Food        = 0,
          Production  = 0,
          TerrainType = terrainRanges.First(range =>
            (noiseMap[x, y] / noiseMax) >= range.Min &&
            (noiseMap[x, y] / noiseMax) <= range.Max
          ).TerrainType
        };

        map.MapData[hex.Coordinates] = hex;
      }
    }
  }
}