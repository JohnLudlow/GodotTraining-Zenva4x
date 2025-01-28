using Godot;

using Zenva4x.Core.Game.Entities.MapEntities;

namespace Zenva4x.UnitTests.Game.Entities.MapEntities;

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

  [Fact]
  public void MapCreate_WithNonZeroDimensions_CreatesRectangularMap()
  {
    var map = MapEntity.Create(100, 100);

    Assert.IsType<MapEntity>(map);
    Assert.Equal(100, map.Width);
    Assert.Equal(100, map.Height);
    Assert.Equal(0, map.NumberOfAIPlayers);
    Assert.Equal(
      (255, 255, 255),
      (map.PlayerCivilizationColor.R, map.PlayerCivilizationColor.G, map.PlayerCivilizationColor.B)
    );
    Assert.Equal(10000, map.MapData.Count);
    Assert.All(map.MapData.Values, h => Assert.Equal(TerrainType.Water, h.TerrainType));
  }
}