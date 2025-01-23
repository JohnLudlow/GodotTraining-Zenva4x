using Godot;

using Zenva4x.GodotProject.Game.Entities.MapEntities;

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
    }
  }
}