using Godot;

using Xunit.Sdk;

using Zenva4x.Core.Game.Entities.MapEntities;
using Zenva4x.Core.Wrappers.GodotWrappers;

namespace Zenva4x.UnitTests.Game.Entities.MapEntities;

public class FastNoiseLiteMock : IFastNoiseLite
{
  public FastNoiseLite.NoiseTypeEnum NoiseType { get; set; }
  public int Seed { get; set; }
  public float Frequency { get; set; }
  public FastNoiseLite.FractalTypeEnum FractalType { get; set; }
  public int FractalOctaves { get; set; }
  public float FractalLacunarity { get; set; }
  public float GetNoise2D(float x, float y) => GetNoise2DFunc?.Invoke(x, y) ?? 0;

  #region mockers
  public Func<float, float, float> GetNoise2DFunc {get;init;}
  #endregion
}

public class MapConstructorTests
{

  [Fact]
  public void MapCreate_WithZeroDimensions_CreatesEmptyMap()
    {
      var map = new MapEntity() {
        Width                   = 0,
        Height                  = 0,
        NumberOfAIPlayers       = 0,
        PlayerCivilizationColor = new Color(255, 255, 255),
      };

      Assert.IsType<MapEntity>(map);
      Assert.Equal(0, map.Width);
      Assert.Equal(0, map.Height);
      Assert.Equal(0, map.NumberOfAIPlayers);
      Assert.Equal(
        (255, 255, 255),
        (map.PlayerCivilizationColor.R, map.PlayerCivilizationColor.G, map.PlayerCivilizationColor.B)
      );
      Assert.Empty(map.MapData);
    }

  [Theory()]
  [InlineData(0, 0, TerrainType.Water)]
  [InlineData(0, 1, TerrainType.Water)]
  [InlineData(0, 2, TerrainType.Water)]
  [InlineData(0, 3, TerrainType.Water)]
  [InlineData(0, 4, TerrainType.Water)]
  [InlineData(0, 5, TerrainType.Water)]
  [InlineData(0, 6, TerrainType.Water)]
  [InlineData(0, 7, TerrainType.Water)]
  [InlineData(1, 0, TerrainType.Water)]
  [InlineData(1, 1, TerrainType.Water)]
  [InlineData(1, 2, TerrainType.Water)]
  [InlineData(1, 3, TerrainType.Water)]
  [InlineData(1, 4, TerrainType.Water)]
  [InlineData(1, 5, TerrainType.Water)]
  [InlineData(1, 6, TerrainType.Water)]
  [InlineData(1, 7, TerrainType.Water)]  
  [InlineData(2, 0, TerrainType.ShallowWater)]
  [InlineData(2, 1, TerrainType.ShallowWater)]
  [InlineData(2, 2, TerrainType.ShallowWater)]
  [InlineData(2, 3, TerrainType.ShallowWater)]
  [InlineData(2, 4, TerrainType.ShallowWater)]
  [InlineData(2, 5, TerrainType.ShallowWater)]
  [InlineData(2, 6, TerrainType.ShallowWater)]
  [InlineData(2, 7, TerrainType.ShallowWater)]
  [InlineData(3, 0, TerrainType.ShallowWater)]
  [InlineData(3, 1, TerrainType.ShallowWater)]
  [InlineData(3, 2, TerrainType.ShallowWater)]
  [InlineData(3, 3, TerrainType.ShallowWater)]
  [InlineData(3, 4, TerrainType.ShallowWater)]
  [InlineData(3, 5, TerrainType.ShallowWater)]
  [InlineData(3, 6, TerrainType.ShallowWater)]
  [InlineData(3, 7, TerrainType.ShallowWater)]
  [InlineData(4, 0, TerrainType.Beach)]
  [InlineData(4, 1, TerrainType.Beach)]
  [InlineData(4, 2, TerrainType.Beach)]
  [InlineData(4, 3, TerrainType.Beach)]
  [InlineData(4, 4, TerrainType.Beach)]
  [InlineData(4, 5, TerrainType.Beach)]
  [InlineData(4, 6, TerrainType.Beach)]
  [InlineData(4, 7, TerrainType.Beach)]
  [InlineData(5, 0, TerrainType.Beach)]
  [InlineData(5, 1, TerrainType.Beach)]
  [InlineData(5, 2, TerrainType.Beach)]
  [InlineData(5, 3, TerrainType.Beach)]
  [InlineData(5, 4, TerrainType.Beach)]
  [InlineData(5, 5, TerrainType.Beach)]
  [InlineData(5, 6, TerrainType.Beach)]
  [InlineData(5, 7, TerrainType.Beach)]
  [InlineData(6, 0, TerrainType.Plains)]
  [InlineData(6, 1, TerrainType.Plains)]
  [InlineData(6, 2, TerrainType.Plains)]
  [InlineData(6, 3, TerrainType.Plains)]
  [InlineData(6, 4, TerrainType.Plains)]
  [InlineData(6, 5, TerrainType.Plains)]
  [InlineData(6, 6, TerrainType.Plains)]
  [InlineData(6, 7, TerrainType.Plains)]
  [InlineData(7, 0, TerrainType.Plains)]
  [InlineData(7, 1, TerrainType.Plains)]
  [InlineData(7, 2, TerrainType.Plains)]
  [InlineData(7, 3, TerrainType.Plains)]
  [InlineData(7, 4, TerrainType.Plains)]
  [InlineData(7, 5, TerrainType.Plains)]
  [InlineData(7, 6, TerrainType.Plains)]
  [InlineData(7, 7, TerrainType.Plains)]

  public void MapCreate_WithNonZeroDimensions_CreatesNoisyMap(int x, int y, TerrainType terrainType)
  {
    var map = MapEntity.Create(new FastNoiseLiteMock()
    {
      GetNoise2DFunc = (x, _) =>
        x switch
        {
          < 2 => .0f,
          < 4 => .25f,
          < 6 => .50f,
          < 8 => .75f,
          _ => 1f,
        }
    }, 8, 8,
    [
      new (0    , .25f, TerrainType.Water),
      new (.25f , .50f, TerrainType.ShallowWater),
      new (.50f , .75f, TerrainType.Beach),
      new (.75f , 1.0f, TerrainType.Plains),
    ]
  );

    Assert.IsType<MapEntity>(map);
    Assert.Equal(8, map.Width);
    Assert.Equal(8, map.Height);
    Assert.Equal(0, map.NumberOfAIPlayers);
    Assert.Equal(
      (255, 255, 255),
      (map.PlayerCivilizationColor.R, map.PlayerCivilizationColor.G, map.PlayerCivilizationColor.B)
    );
    
    Assert.Equal(64, map.MapData.Count);

    Assert.Equal(new Vector2I(x, y), map.MapData[new Vector2I(x, y)].Coordinates);
    Assert.Equal(terrainType,        map.MapData[new Vector2I(x, y)].TerrainType);

    Assert.All(map.MapData, h =>
    {
      Assert.Equal(h.Key.X, h.Value.Coordinates.X);
      Assert.Equal(h.Key.Y, h.Value.Coordinates.Y);
      
      Assert.Equal(0, h.Value.Food);
      Assert.Equal(0, h.Value.Production);
    });
  }  
}