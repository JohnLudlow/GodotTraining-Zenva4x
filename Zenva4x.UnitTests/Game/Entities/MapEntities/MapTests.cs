using Zenva4x.GodotProject.Game.Entities.MapEntities;

namespace Zenva4x.UnitTests.Game.Entities.MapEntities
{
  public static class MapTests
  {
    public class Constructor
    {
      [Fact]
      public void CreatesMap()
      {
        var hex = new MapEntity();

        Assert.IsType<MapEntity>(hex);
      }
    }
  }
}