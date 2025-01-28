using Godot;

using Zenva4x.Core.Game.Entities.MapEntities;

namespace Zenva4x.UnitTests.Game.Entities.MapEntities;

public static class MapHexTests
{
  public class Constructor
  {
    [Fact]
    public void CreatesHex()
    {
      var hex = new MapHexEntity{
        TerrainType = TerrainType.Water,
        Coordinates = Vector2I.Zero,
        Food        = 0,
        Production  = 0,
      };

      Assert.IsType<MapHexEntity>(hex);
      Assert.Equal(TerrainType.Water, hex.TerrainType);

      Assert.Equal(0, hex.Coordinates.X);
      Assert.Equal(0, hex.Coordinates.Y);

      Assert.Equal(0, hex.Food);
      Assert.Equal(0, hex.Production);
    }
  }
}