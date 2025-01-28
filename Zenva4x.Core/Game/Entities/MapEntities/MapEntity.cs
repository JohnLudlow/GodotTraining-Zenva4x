using Godot;

using Zenva4x.Core.Wrappers.GodotWrappers;

namespace Zenva4x.Core.Game.Entities.MapEntities;

public class FastNoiseLiteMock : IFastNoiseLite
{
  public FastNoiseLite.NoiseTypeEnum NoiseType { get; set; }
  public int Seed { get; set; }
  public float Frequency { get; set; }
  public FastNoiseLite.FractalTypeEnum FractalType { get; set; }
  public int FractalOctaves { get; set; }
  public float FractalLacunarity { get; set; }

  public float GetNoise2D(float x, float y) => 0f;
}

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

  public static MapEntity Create(IFastNoiseLite fastNoiseLite, int width, int height)
  {
    var map = new MapEntity()
    {
      Width = width,
      Height = height,
      NumberOfAIPlayers = 0,
      PlayerCivilizationColor = new Color(255, 255, 255),
    };

    var rand = new Random();
    var seed = rand.Next(100000);

    var (baseNoiseMax, baseNoiseMap) = MakeSomeNoise(map, fastNoiseLite);
    
    var baseTerrainRanges = new [] {
      (0                        , Math.Clamp(baseNoiseMax / 10 * 2.5f, .01f, 1f), TerrainType.Water),
      (baseNoiseMax / 10 * 2.5f , Math.Clamp(baseNoiseMax / 10 * 4f  , 1f,   2f), TerrainType.ShallowWater),
      (baseNoiseMax / 10 * 4f   , Math.Clamp(baseNoiseMax / 10 * 4.5f, 2f,   3f), TerrainType.Beach),
      (baseNoiseMax / 10 * 4.5f , Math.Clamp(baseNoiseMax + .05f     , 3f,   4f), TerrainType.Plains)
    };

    DrawTerrain(map, baseNoiseMap, baseTerrainRanges);

    return map;
  }

  private static (float noiseMax, float[,] noiseMap) MakeSomeNoise(MapEntity map, IFastNoiseLite noise)
  {
    var noiseMap = new float[map.Width, map.Height];
    var noiseMax = 0f;

    for (var x = 0; x < map.Width; x++)
    {
      for (var y = 0; y < map.Height; y++)
      {
        noiseMap[x, y] = Math.Abs(noise.GetNoise2D(x, y));
        if (noiseMap[x, y] > noiseMax)
        {
          noiseMax = noiseMap[x, y];
        }
      }
    }

    return (noiseMax, noiseMap);
  }

  private static void DrawTerrain(MapEntity map, float[,] noiseMap, IEnumerable<(float min, float max, TerrainType terrainType)> terrainRanges)
  {
    for (var x = 0; x < map.Width; x++)
    {
      for (var y = 0; y < map.Height; y++)
      {
        var hex = new MapHexEntity{
          Coordinates = new Vector2I(x, y),
          Food        = 0,
          Production  = 0,
          TerrainType = terrainRanges.First(range =>
            noiseMap[x, y] >= range.min &&
            noiseMap[x, y] < range.max
        ).terrainType
        };

        map.MapData[hex.Coordinates] = hex;
      }
    }
  }
}